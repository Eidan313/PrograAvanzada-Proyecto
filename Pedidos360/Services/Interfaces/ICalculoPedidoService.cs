using Pedidos360.ViewModels.Pedidos;

namespace Pedidos360.Services.Interfaces;

public interface ICalculoPedidoService
{
    Task<ResultadoCalculoDto> CalcularAsync(CalcularPedidoEntrada entrada);
}
