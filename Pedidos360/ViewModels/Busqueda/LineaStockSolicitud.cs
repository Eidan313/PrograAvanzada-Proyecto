using System.ComponentModel.DataAnnotations;

namespace Pedidos360.ViewModels.Busqueda;

// Una línea que se quiere vender: producto + cantidad.
public class LineaStockSolicitud
{
    [Range(1, int.MaxValue, ErrorMessage = "El producto es requerido.")]
    public int ProductoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
    public int Cantidad { get; set; }
}
