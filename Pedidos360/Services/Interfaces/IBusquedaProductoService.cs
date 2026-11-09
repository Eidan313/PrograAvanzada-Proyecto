using Pedidos360.ViewModels.Busqueda;

namespace Pedidos360.Services.Interfaces;

public interface IBusquedaProductoService
{
    // Busca productos activos por nombre o código (autocompletado).
    Task<List<ProductoBusquedaDto>> BuscarAsync(string? texto, int maximoResultados = 10);

    // Valida si hay stock suficiente de un producto.
    Task<ValidacionStockDto> ValidarStockAsync(int productoId, int cantidad);

    // Valida todas las líneas de un pedido; suma las cantidades del mismo producto.
    Task<ResultadoValidacionStockDto> ValidarStockAsync(IEnumerable<LineaStockSolicitud> lineas);
}
