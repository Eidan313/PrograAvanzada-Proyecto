using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Models;
using Pedidos360.Services.Interfaces;
namespace Pedidos360.Services;
public class ProductoService : IProductoService
{
 private readonly ApplicationDbContext _db;
 public ProductoService(ApplicationDbContext db) => _db=db;
 public async Task<(List<Producto> productos,int total)> ListarAsync(string? nombre,int? categoriaId,int pagina)
 {
  var q=_db.Set<Producto>().Include(x=>x.Categoria).AsNoTracking().AsQueryable();
  if(!string.IsNullOrWhiteSpace(nombre)) q=q.Where(x=>x.Nombre.Contains(nombre));
  if(categoriaId.HasValue) q=q.Where(x=>x.CategoriaId==categoriaId.Value);
  var total=await q.CountAsync();
  var productos=await q.OrderBy(x=>x.Nombre).ThenBy(x=>x.Id).Skip((Math.Max(1,pagina)-1)*10).Take(10).ToListAsync();
  return (productos,total);
 }
 public Task<Producto?> ObtenerAsync(int id)=>_db.Set<Producto>().FirstOrDefaultAsync(x=>x.Id==id);
 public Task<List<Categoria>> CategoriasAsync()=>_db.Set<Categoria>().OrderBy(x=>x.Nombre).AsNoTracking().ToListAsync();
 public async Task CrearAsync(Producto producto){_db.Set<Producto>().Add(producto);await _db.SaveChangesAsync();}
 public async Task ActualizarAsync(Producto producto){_db.Set<Producto>().Update(producto);await _db.SaveChangesAsync();}
 public async Task<bool> EliminarAsync(int id)
 {
  var producto=await _db.Set<Producto>().FindAsync(id);
  if(producto==null)return false;
  if(await _db.Set<PedidoDetalle>().AnyAsync(x=>x.ProductoId==id))return false;
  _db.Set<Producto>().Remove(producto);await _db.SaveChangesAsync();return true;
 }
}
