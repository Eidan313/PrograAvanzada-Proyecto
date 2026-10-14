namespace Pedidos360.ViewModels
{
    // Una fila del listado de categorias, con la cantidad de productos que tiene.
    public class CategoriaFilaViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int CantidadProductos { get; set; }
    }
}
