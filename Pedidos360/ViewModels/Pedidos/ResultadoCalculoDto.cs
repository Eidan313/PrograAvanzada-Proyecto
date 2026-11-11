using Pedidos360.Models;

namespace Pedidos360.ViewModels.Pedidos;

// Resultado de una linea.
public class LineaCalculadaDto
{
    public int ProductoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Cantidad { get; set; }
    public decimal PrecioUnit { get; set; }
    public int StockDisponible { get; set; }
    public TipoDescuento TipoDescuento { get; set; }

    // Descuento escrito
    public decimal Descuento { get; set; }

    // Descuento convertido a moneda
    public decimal DescuentoMonto { get; set; }
    public decimal ImpuestoPorc { get; set; }

    // Cantidad x precio - descuento
    public decimal Subtotal { get; set; }
    public decimal Impuesto { get; set; }
    public decimal TotalLinea { get; set; }
}

public class ResultadoCalculoDto
{
    public List<LineaCalculadaDto> Lineas { get; set; } = [];
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Total { get; set; }
}
