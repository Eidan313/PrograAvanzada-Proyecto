using Pedidos360.Models;
namespace Pedidos360.Services.Interfaces;
public interface IProductoService
{
 Task<(List<Producto> productos,int total)> ListarAsync(string? nombre,int? categoriaId,int pagina);
 Task<Producto?> ObtenerAsync(int id);
 Task<List<Categoria>> CategoriasAsync();
 Task CrearAsync(Producto producto);
 Task ActualizarAsync(Producto producto);
 Task<bool> EliminarAsync(int id);
}
