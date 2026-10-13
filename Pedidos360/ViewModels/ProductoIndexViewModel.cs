using Pedidos360.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Pedidos360.ViewModels;
public class ProductoIndexViewModel
{
 public List<Producto> Productos {get;set;} = new();
 public string? Nombre {get;set;}
 public int? CategoriaId {get;set;}
 public int Pagina {get;set;} = 1;
 public int TotalPaginas {get;set;}
 public List<SelectListItem> Categorias {get;set;} = new();
}
