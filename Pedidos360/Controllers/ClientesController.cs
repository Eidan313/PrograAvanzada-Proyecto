using Microsoft.AspNetCore.Mvc;

namespace Pedidos360.Controllers
{
    public class ClientesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
