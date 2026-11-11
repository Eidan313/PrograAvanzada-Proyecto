namespace Pedidos360.ViewModels.Busqueda;

// Resultado liviano del autocompletado: no se envía la imagen en base64, solo su URL.
public class ProductoBusquedaDto
{
    public int Id { get; set; }

    public string Codigo { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string? Categoria { get; set; }

    public decimal Precio { get; set; }

    public decimal ImpuestoPorc { get; set; }

    public int Stock { get; set; }

    public bool HayStock => Stock > 0;

    public string ImagenUrl { get; set; } = string.Empty;
}
