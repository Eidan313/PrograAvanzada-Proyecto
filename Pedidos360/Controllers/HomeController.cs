using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pedidos360.Models;

namespace Pedidos360.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() => View();

        public IActionResult Privacy() => View();

        //-----------
        // Prueba QA
       // public IActionResult ProvocarError()
     //   {
        //    throw new InvalidOperationException("Prueba QA: 500 provocado a propósito");
      //  }
        //-----------

        // Excepción no controlada 500 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var fallo = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError(fallo?.Error,
                "Error no controlado en {Ruta}. Referencia {Referencia}",
                fallo?.Path, HttpContext.TraceIdentifier);

            Response.StatusCode = StatusCodes.Status500InternalServerError;
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        // Códigos de estado sin cuerpo 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Estado(int id)
        {
            var original = HttpContext.Features.Get<IStatusCodeReExecuteFeature>()?.OriginalPath;
            Response.StatusCode = id;

            if (id == StatusCodes.Status404NotFound)
            {
                _logger.LogWarning("Recurso no encontrado: {Ruta}. Referencia {Referencia}",
                    original, HttpContext.TraceIdentifier);
                return View("NoEncontrado", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }

            _logger.LogWarning("Código {Codigo} en {Ruta}. Referencia {Referencia}",
                id, original, HttpContext.TraceIdentifier);
            return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }
    }
}