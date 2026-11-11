using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;

namespace Pedidos360.Controllers;

[Authorize(Roles = "Admin,Vendedor")]
public class PedidosController : Controller
{
    private readonly ApplicationDbContext _db;
    public PedidosController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var pedidos = await _db.Pedidos.AsNoTracking()
            .Include(p => p.Cliente)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
        return View(pedidos);
    }

    public async Task<IActionResult> Details(int id)
    {
        var pedido = await _db.Pedidos.AsNoTracking()
            .Include(p => p.Cliente)
            .Include(p => p.Detalles).ThenInclude(d => d.Producto)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (pedido is null) return NotFound();
        return View(pedido);
    }
}
