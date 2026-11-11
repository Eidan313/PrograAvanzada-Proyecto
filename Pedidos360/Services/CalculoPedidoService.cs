using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Models;
using Pedidos360.Services.Interfaces;
using Pedidos360.ViewModels.Pedidos;

namespace Pedidos360.Services;

public class CalculoPedidoService(ApplicationDbContext db) : ICalculoPedidoService
{
    public async Task<ResultadoCalculoDto> CalcularAsync(CalcularPedidoEntrada entrada)
    {
        if (entrada.Lineas.Count == 0) throw new ReglaNegocioException(422, "El pedido necesita al menos un producto.");

        var ids = entrada.Lineas.Select(l => l.ProductoId).Distinct().ToList();

        var productos = await db.Productos.AsNoTracking()
            .Where(p => ids.Contains(p.Id) && p.Activo)
            .Select(p => new { p.Id, p.Codigo, p.Nombre, p.Precio, p.ImpuestoPorc, p.Stock })
            .ToDictionaryAsync(p => p.Id);

        var lineas = new List<LineaCalculadaDto>();

        // El mismo producto en varias lineas se une
        foreach (var grupo in entrada.Lineas.GroupBy(l => l.ProductoId))
        {
            var producto = productos.GetValueOrDefault(grupo.Key)
                ?? throw new ReglaNegocioException(404, "Uno de los productos ya no esta disponible.");

            if (grupo.Select(l => (l.TipoDescuento, l.Descuento)).Distinct().Count() > 1)
                throw new ReglaNegocioException(422, $"El producto {producto.Nombre} esta repetido con descuentos distintos.");

            int cantidad = grupo.Sum(l => l.Cantidad);
            if (cantidad < 1 || cantidad > 100) throw new ReglaNegocioException(422, "La cantidad por producto debe estar entre 1 y 100.");
            if (producto.Stock < cantidad)
                throw new ReglaNegocioException(409, $"Solo quedan {producto.Stock} unidades de {producto.Nombre}.");

            // El precio y el impuesto vienen del servidor
            var primera = grupo.First();
            var linea = CalculadoraPedido.CalcularLinea(cantidad, producto.Precio, producto.ImpuestoPorc, primera.TipoDescuento, primera.Descuento);
            linea.ProductoId = producto.Id;
            linea.Codigo = producto.Codigo;
            linea.Nombre = producto.Nombre;
            linea.StockDisponible = producto.Stock;
            lineas.Add(linea);
        }

        return CalculadoraPedido.CalcularTotales(lineas);
    }
}
