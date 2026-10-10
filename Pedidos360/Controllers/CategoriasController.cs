using Microsoft.AspNetCore.Mvc;

namespace Pedidos360.Controllers
{
    public class CategoriasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
