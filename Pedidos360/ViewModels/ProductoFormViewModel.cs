using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace Pedidos360.ViewModels;
public class ProductoFormViewModel
{
 public int Id {get;set;}
 [Required(ErrorMessage="El código es requerido.")][StringLength(20)] public string Codigo {get;set;} = "";
 [Required(ErrorMessage="El nombre es requerido.")][StringLength(100)] public string Nombre {get;set;} = "";
 [Range(1,int.MaxValue,ErrorMessage="Seleccione una categoría.")] public int CategoriaId {get;set;}
 [Range(typeof(decimal),"0.01","9999999999999999.99",ErrorMessage="El precio debe ser mayor a 0.")] public decimal Precio {get;set;}
 [Range(0,int.MaxValue,ErrorMessage="El stock no puede ser negativo.")] public int Stock {get;set;}
 [Range(typeof(decimal),"0","100",ErrorMessage="El impuesto debe estar entre 0 y 100.")] public decimal ImpuestoPorc {get;set;}
 public bool Activo {get;set;} = true;
 public IFormFile? ImagenArchivo {get;set;}
 public string? ImagenActual {get;set;}
 public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Categorias {get;set;} = new();
}
