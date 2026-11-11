using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pedidos360.Models;
using Pedidos360.Services.Interfaces;

namespace Pedidos360.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClientesController : Controller
    {
        private readonly IClienteService _clienteService;

        public ClientesController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        public async Task<IActionResult> Index(string? busqueda, int pagina = 1)
        {
            return View(await _clienteService.ObtenerPaginaAsync(busqueda, pagina));
        }

        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            return cliente is null ? NotFound() : View(cliente);
        }

        public IActionResult Create()
        {
            return View(new Cliente());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            ValidarTipoIdentificacion(cliente);
            Normalizar(cliente);

            if (ModelState.IsValid && await _clienteService.ExisteCedulaAsync(cliente.Cedula))
            {
                ModelState.AddModelError(nameof(cliente.Cedula), "Ya existe un cliente con esta cédula.");
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            if (!await _clienteService.CrearAsync(cliente))
            {
                ModelState.AddModelError(nameof(cliente.Cedula), "Ya existe un cliente con esta cédula.");
                return View(cliente);
            }

            TempData["Exito"] = "Cliente creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            return cliente is null ? NotFound() : View(cliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            ValidarTipoIdentificacion(cliente);
            Normalizar(cliente);

            if (ModelState.IsValid && await _clienteService.ExisteCedulaAsync(cliente.Cedula, cliente.Id))
            {
                ModelState.AddModelError(nameof(cliente.Cedula), "Ya existe un cliente con esta cédula.");
            }

            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            if (!await _clienteService.ActualizarAsync(cliente))
            {
                ModelState.AddModelError(nameof(cliente.Cedula), "Ya existe un cliente con esta cédula.");
                return View(cliente);
            }

            TempData["Exito"] = "Cliente actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            if (cliente is null)
            {
                return NotFound();
            }

            ViewBag.TienePedidos = await _clienteService.TienePedidosAsync(id);
            return View(cliente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await _clienteService.EliminarSiNoTienePedidosAsync(id))
            {
                TempData["Error"] = "No se puede eliminar el cliente porque tiene pedidos asociados o ya no existe.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Exito"] = "Cliente eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        private void ValidarTipoIdentificacion(Cliente cliente)
        {
            if (!Enum.IsDefined(cliente.TipoIdentificacion))
            {
                ModelState.AddModelError(nameof(cliente.TipoIdentificacion), "Seleccione un tipo de identificación válido.");
            }
        }

        private static void Normalizar(Cliente cliente)
        {
            cliente.Cedula = cliente.Cedula?.Trim() ?? string.Empty;
            cliente.Nombre = cliente.Nombre?.Trim() ?? string.Empty;
            cliente.Correo = cliente.Correo?.Trim() ?? string.Empty;
            cliente.Telefono = cliente.Telefono?.Trim() ?? string.Empty;
            cliente.Direccion = cliente.Direccion?.Trim() ?? string.Empty;
        }
    }
}
