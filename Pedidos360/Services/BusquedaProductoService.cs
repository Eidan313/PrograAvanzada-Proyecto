using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Services.Interfaces;
using Pedidos360.ViewModels.Busqueda;

namespace Pedidos360.Services;

public class BusquedaProductoService : IBusquedaProductoService
{
    public const int MinimoCaracteres = 2;
    private const int MaximoPermitido = 20;

    private readonly ApplicationDbContext _db;

    public BusquedaProductoService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<ProductoBusquedaDto>> BuscarAsync(string? texto, int maximoResultados = 10)
    {
        string textoABuscar = (texto ?? string.Empty).Trim();
        if (textoABuscar.Length < MinimoCaracteres)
        {
            return new List<ProductoBusquedaDto>();
        }

        int cantidadDeResultados = Math.Clamp(maximoResultados, 1, MaximoPermitido);

        // Solo productos activos. Primero los que empiezan con el texto y luego los que lo contienen.
        return await _db.Productos
            .AsNoTracking()
            .Where(producto => producto.Activo
                && (producto.Nombre.Contains(textoABuscar) || producto.Codigo.Contains(textoABuscar)))
            .OrderByDescending(producto => producto.Nombre.StartsWith(textoABuscar) || producto.Codigo.StartsWith(textoABuscar))
            .ThenBy(producto => producto.Nombre)
            .Take(cantidadDeResultados)
            .Select(producto => new ProductoBusquedaDto
            {
                Id = producto.Id,
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                Categoria = producto.Categoria != null ? producto.Categoria.Nombre : null,
                Precio = producto.Precio,
                ImpuestoPorc = producto.ImpuestoPorc,
                Stock = producto.Stock,
                ImagenUrl = "/Productos/Imagen/" + producto.Id
            })
            .ToListAsync();
    }

    public async Task<ValidacionStockDto> ValidarStockAsync(int productoId, int cantidad)
    {
        ResultadoValidacionStockDto resultado = await ValidarStockAsync(new[]
        {
            new LineaStockSolicitud { ProductoId = productoId, Cantidad = cantidad }
        });

        return resultado.Lineas.First();
    }

    public async Task<ResultadoValidacionStockDto> ValidarStockAsync(IEnumerable<LineaStockSolicitud> lineas)
    {
        // Si el mismo producto viene en varias líneas, se valida la suma.
        var cantidadesPorProducto = lineas
            .GroupBy(linea => linea.ProductoId)
            .Select(grupo => new { ProductoId = grupo.Key, Cantidad = grupo.Sum(linea => linea.Cantidad) })
            .ToList();

        List<int> idsDeProductos = cantidadesPorProducto.Select(linea => linea.ProductoId).ToList();

        var productosEnBaseDeDatos = await _db.Productos
            .AsNoTracking()
            .Where(producto => idsDeProductos.Contains(producto.Id))
            .Select(producto => new { producto.Id, producto.Nombre, producto.Stock, producto.Activo })
            .ToDictionaryAsync(producto => producto.Id);

        var resultado = new ResultadoValidacionStockDto();

        foreach (var linea in cantidadesPorProducto)
        {
            var validacion = new ValidacionStockDto
            {
                ProductoId = linea.ProductoId,
                CantidadSolicitada = linea.Cantidad
            };

            if (!productosEnBaseDeDatos.TryGetValue(linea.ProductoId, out var producto))
            {
                validacion.Mensaje = "El producto seleccionado no existe.";
            }
            else
            {
                validacion.Producto = producto.Nombre;
                validacion.StockDisponible = producto.Stock;

                if (!producto.Activo)
                {
                    validacion.Mensaje = $"El producto \"{producto.Nombre}\" está inactivo y no se puede vender.";
                }
                else if (linea.Cantidad <= 0)
                {
                    validacion.Mensaje = "La cantidad debe ser mayor a 0.";
                }
                else if (linea.Cantidad > producto.Stock)
                {
                    validacion.Mensaje = $"Stock insuficiente para \"{producto.Nombre}\": solicitó {linea.Cantidad} y hay {producto.Stock} disponible(s).";
                }
                else
                {
                    validacion.Disponible = true;
                    validacion.Mensaje = $"Hay stock suficiente ({producto.Stock} disponible(s)).";
                }
            }

            resultado.Lineas.Add(validacion);
        }

        return resultado;
    }
}
