using Pedidos360.Models;
using Pedidos360.ViewModels;

namespace Pedidos360.Services.Interfaces
{
    public interface ICategoriaService
    {
        Task<ListaPaginada<CategoriaFilaViewModel>> ObtenerPaginaAsync(int pagina);
        Task<Categoria?> ObtenerPorIdAsync(int id);
        Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null);
        Task<bool> CrearAsync(Categoria categoria);
        Task<bool> ActualizarAsync(Categoria categoria);
        Task<int> ContarProductosAsync(int id);
        Task<bool> EliminarSiNoTieneProductosAsync(int id);
    }
}
