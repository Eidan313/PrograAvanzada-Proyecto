using Microsoft.AspNetCore.Mvc;
namespace Pedidos360.Controllers;
public class AccesoController : Controller
{
    [HttpGet]
    public IActionResult Denegado() => View();
}
