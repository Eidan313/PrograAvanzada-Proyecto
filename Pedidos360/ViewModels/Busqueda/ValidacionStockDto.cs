namespace Pedidos360.ViewModels.Busqueda;

// Resultado de validar el stock de un producto.
public class ValidacionStockDto
{
    public int ProductoId { get; set; }

    public string Producto { get; set; } = string.Empty;

    public int CantidadSolicitada { get; set; }

    public int StockDisponible { get; set; }

    public bool Disponible { get; set; }

    public string Mensaje { get; set; } = string.Empty;
}

// Resultado de validar todas las líneas de un pedido.
public class ResultadoValidacionStockDto
{
    public bool EsValido => Lineas.All(linea => linea.Disponible);

    public List<ValidacionStockDto> Lineas { get; set; } = new();

    public IEnumerable<string> Errores => Lineas.Where(linea => !linea.Disponible).Select(linea => linea.Mensaje);
}
