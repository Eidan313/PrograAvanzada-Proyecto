using Microsoft.AspNetCore.Mvc;

namespace Pedidos360.Controllers
{
    public class ProductosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
