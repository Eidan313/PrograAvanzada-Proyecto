using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pedidos360.Services;
using Pedidos360.Services.Interfaces;
using Pedidos360.ViewModels.Busqueda;

namespace Pedidos360.Controllers.Api;

// Endpoints JSON del módulo de productos. Solo usuarios autenticados.
[ApiController]
[Authorize]
[Route("api/productos")]
public class ProductosApiController : ControllerBase
{
    private readonly IBusquedaProductoService _busquedaProductoService;
    private readonly ILogger<ProductosApiController> _logger;

    public ProductosApiController(IBusquedaProductoService busquedaProductoService, ILogger<ProductosApiController> logger)
    {
        _busquedaProductoService = busquedaProductoService;
        _logger = logger;
    }

    // GET /api/productos/buscar?q=lap&max=10
    [HttpGet("buscar")]
    public async Task<ActionResult<List<ProductoBusquedaDto>>> Buscar([FromQuery] string? q, [FromQuery] int max = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < BusquedaProductoService.MinimoCaracteres)
            {
                return Ok(new List<ProductoBusquedaDto>());
            }

            List<ProductoBusquedaDto> losProductosEncontrados = await _busquedaProductoService.BuscarAsync(q, max);
            return Ok(losProductosEncontrados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar productos con el texto {Texto}", q);
            return Problem("No se pudo realizar la búsqueda de productos.");
        }
    }

    // GET /api/productos/5/stock?cantidad=3
    [HttpGet("{id:int}/stock")]
    public async Task<ActionResult<ValidacionStockDto>> ConsultarStock(int id, [FromQuery] int cantidad = 1)
    {
        try
        {
            ValidacionStockDto laValidacion = await _busquedaProductoService.ValidarStockAsync(id, cantidad);
            return Ok(laValidacion);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar el stock del producto {ProductoId}", id);
            return Problem("No se pudo consultar el stock del producto.");
        }
    }

    // POST /api/productos/validar-stock   (requiere el token antiforgery en el encabezado)
    // Body: [ { "productoId": 1, "cantidad": 2 }, ... ]
    [HttpPost("validar-stock")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult<ResultadoValidacionStockDto>> ValidarStock([FromBody] List<LineaStockSolicitud> lineas)
    {
        try
        {
            if (lineas == null || lineas.Count == 0)
            {
                return BadRequest(new { mensaje = "Debe enviar al menos una línea." });
            }

            ResultadoValidacionStockDto elResultado = await _busquedaProductoService.ValidarStockAsync(lineas);
            return Ok(elResultado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar el stock de un pedido");
            return Problem("No se pudo validar el stock.");
        }
    }
}
