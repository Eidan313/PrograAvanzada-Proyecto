using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pedidos360.Controllers;

// Página de demostración de la búsqueda AJAX y la validación de stock en vivo.
[Authorize]
public class BusquedaController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
