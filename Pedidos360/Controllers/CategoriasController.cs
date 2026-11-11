using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pedidos360.Models;
using Pedidos360.Services.Interfaces;

namespace Pedidos360.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriasController : Controller
    {
        private const string MensajeNombreDuplicado = "Ya existe una categoría con ese nombre.";

        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        public async Task<IActionResult> Index(int pagina = 1)
        {
            return View(await _categoriaService.ObtenerPaginaAsync(pagina));
        }

        public async Task<IActionResult> Details(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria is null)
            {
                return NotFound();
            }

            ViewBag.CantidadProductos = await _categoriaService.ContarProductosAsync(id);
            return View(categoria);
        }

        public IActionResult Create()
        {
            return View(new Categoria());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre")] Categoria categoria)
        {
            Normalizar(categoria);

            if (ModelState.IsValid && await _categoriaService.ExisteNombreAsync(categoria.Nombre))
            {
                ModelState.AddModelError(nameof(categoria.Nombre), MensajeNombreDuplicado);
            }

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            if (!await _categoriaService.CrearAsync(categoria))
            {
                ModelState.AddModelError(nameof(categoria.Nombre), MensajeNombreDuplicado);
                return View(categoria);
            }

            TempData["Exito"] = "Categoría creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            return categoria is null ? NotFound() : View(categoria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest();
            }

            Normalizar(categoria);

            if (ModelState.IsValid && await _categoriaService.ExisteNombreAsync(categoria.Nombre, categoria.Id))
            {
                ModelState.AddModelError(nameof(categoria.Nombre), MensajeNombreDuplicado);
            }

            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            if (!await _categoriaService.ActualizarAsync(categoria))
            {
                // Puede ser nombre duplicado o que la categoria ya no exista.
                if (await _categoriaService.ObtenerPorIdAsync(id) is null)
                {
                    return NotFound();
                }

                ModelState.AddModelError(nameof(categoria.Nombre), MensajeNombreDuplicado);
                return View(categoria);
            }

            TempData["Exito"] = "Categoría actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _categoriaService.ObtenerPorIdAsync(id);
            if (categoria is null)
            {
                return NotFound();
            }

            // Con productos asociados ni siquiera se muestra la confirmacion: se avisa en _Alertas.
            var cantidadProductos = await _categoriaService.ContarProductosAsync(id);
            if (cantidadProductos > 0)
            {
                TempData["Error"] = $"No se puede eliminar la categoría \"{categoria.Nombre}\" porque tiene {cantidadProductos} producto(s) asociado(s).";
                return RedirectToAction(nameof(Index));
            }

            return View(categoria);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await _categoriaService.EliminarSiNoTieneProductosAsync(id))
            {
                TempData["Error"] = "No se puede eliminar la categoría porque tiene productos asociados o ya no existe.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Exito"] = "Categoría eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private static void Normalizar(Categoria categoria)
        {
            categoria.Nombre = categoria.Nombre?.Trim() ?? string.Empty;
        }
    }
}
