using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pedidos360.Models;
using Pedidos360.Services.Interfaces;
using Pedidos360.ViewModels.Pedidos;

namespace Pedidos360.Controllers.Api;

// Endpoints JSON de pedidos, solo usuarios autenticados
[ApiController]
[Authorize]
[Route("api/pedidos")]
public class PedidosApiController(ICalculoPedidoService calculo) : ControllerBase
{
    // POST /api/pedidos/calcular
    [HttpPost("calcular")]
    public async Task<IActionResult> Calcular(CalcularPedidoEntrada entrada)
    {
        try
        {
            return Ok(await calculo.CalcularAsync(entrada));
        }
        catch (ReglaNegocioException error)
        {
            // Mismo formato de error de la API del profe
            return new ObjectResult(new ProblemDetails
            {
                Status = error.Estado,
                Title = error.Message,
                Extensions = { ["traceId"] = HttpContext.TraceIdentifier }
            }) { StatusCode = error.Estado };
        }
    }
}
