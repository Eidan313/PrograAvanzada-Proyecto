using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pedidos360.Models;
using Pedidos360.Services.Interfaces;
using Pedidos360.ViewModels;
namespace Pedidos360.Controllers;
public class ProductosController : Controller
{
 private readonly IProductoService _servicio;
 public ProductosController(IProductoService servicio)=>_servicio=servicio;
 public async Task<IActionResult> Index(string? nombre,int? categoriaId,int pagina=1)
 {
  pagina=Math.Max(1,pagina);
  var (productos,total)=await _servicio.ListarAsync(nombre,categoriaId,pagina);
  var vm=new ProductoIndexViewModel{Productos=productos,Nombre=nombre,CategoriaId=categoriaId,Pagina=pagina,TotalPaginas=(int)Math.Ceiling(total/10.0),Categorias=await Opciones()};
  return View(vm);
 }
 private async Task<List<SelectListItem>> Opciones()=> (await _servicio.CategoriasAsync()).Select(x=>new SelectListItem(x.Nombre,x.Id.ToString())).ToList();
 public async Task<IActionResult> Create()=>View(new ProductoFormViewModel{Categorias=await Opciones()});
 [HttpPost][ValidateAntiForgeryToken]
 public async Task<IActionResult> Create(ProductoFormViewModel vm)
 {
  ValidarImagen(vm,true);
  if(!ModelState.IsValid){vm.Categorias=await Opciones();return View(vm);}
  var p=Mapear(vm,new Producto());
  await CopiarImagen(vm.ImagenArchivo!,p);
  await _servicio.CrearAsync(p);TempData["Exito"]="Producto creado correctamente.";return RedirectToAction(nameof(Index));
 }
 public async Task<IActionResult> Edit(int id)
 {
  var p=await _servicio.ObtenerAsync(id);if(p==null)return NotFound();
  return View(new ProductoFormViewModel{Id=p.Id,Codigo=p.Codigo,Nombre=p.Nombre,CategoriaId=p.CategoriaId,Precio=p.Precio,Stock=p.Stock,ImpuestoPorc=p.ImpuestoPorc,Activo=p.Activo,ImagenActual=ImagenData(p),Categorias=await Opciones()});
 }
 [HttpPost][ValidateAntiForgeryToken]
 public async Task<IActionResult> Edit(int id,ProductoFormViewModel vm)
 {
  if(id!=vm.Id)return BadRequest();
  var p=await _servicio.ObtenerAsync(id);if(p==null)return NotFound();
  ValidarImagen(vm,false);
  if(!ModelState.IsValid){vm.ImagenActual=ImagenData(p);vm.Categorias=await Opciones();return View(vm);}
  Mapear(vm,p);
  if(vm.ImagenArchivo!=null)await CopiarImagen(vm.ImagenArchivo,p);
  await _servicio.ActualizarAsync(p);TempData["Exito"]="Producto actualizado correctamente.";return RedirectToAction(nameof(Index));
 }
 public async Task<IActionResult> Details(int id){var p=await _servicio.ObtenerAsync(id);return p==null?NotFound():View(p);}
 public async Task<IActionResult> Delete(int id){var p=await _servicio.ObtenerAsync(id);return p==null?NotFound():View(p);}
 [HttpPost,ActionName("Delete")][ValidateAntiForgeryToken]
 public async Task<IActionResult> DeleteConfirmed(int id)
 {
  var p=await _servicio.ObtenerAsync(id);if(p==null)return NotFound();
  if(!await _servicio.EliminarAsync(id))TempData["Error"]="No se puede eliminar el producto porque tiene pedidos asociados.";
  else TempData["Exito"]="Producto eliminado correctamente.";
  return RedirectToAction(nameof(Index));
 }
 public async Task<IActionResult> Imagen(int id)
 {
  var p=await _servicio.ObtenerAsync(id);
  return p==null||p.Imagen.Length==0?NotFound():File(p.Imagen,p.ImagenTipo);
 }
 private void ValidarImagen(ProductoFormViewModel vm,bool obligatoria)
 {
  if(vm.ImagenArchivo==null){if(obligatoria)ModelState.AddModelError(nameof(vm.ImagenArchivo),"La imagen es obligatoria.");return;}
  var img=vm.ImagenArchivo;
  var ext=Path.GetExtension(img.FileName).ToLowerInvariant();
  if(!((ext==".jpg"||ext==".jpeg")&&img.ContentType=="image/jpeg" || ext==".png"&&img.ContentType=="image/png"))ModelState.AddModelError(nameof(vm.ImagenArchivo),"Solo se permiten JPG o PNG.");
  if(img.Length==0||img.Length>2*1024*1024)ModelState.AddModelError(nameof(vm.ImagenArchivo),"La imagen debe tener máximo 2 MB y no estar vacía.");
 }
 private static Producto Mapear(ProductoFormViewModel vm,Producto p){p.Codigo=vm.Codigo;p.Nombre=vm.Nombre;p.CategoriaId=vm.CategoriaId;p.Precio=vm.Precio;p.Stock=vm.Stock;p.ImpuestoPorc=vm.ImpuestoPorc;p.Activo=vm.Activo;return p;}
 private static async Task CopiarImagen(IFormFile archivo,Producto p){using var ms=new MemoryStream();await archivo.CopyToAsync(ms);p.Imagen=ms.ToArray();p.ImagenTipo=archivo.ContentType;}
 private static string? ImagenData(Producto p)=>p.Imagen.Length>0?$"data:{p.ImagenTipo};base64,{Convert.ToBase64String(p.Imagen)}":null;
}
