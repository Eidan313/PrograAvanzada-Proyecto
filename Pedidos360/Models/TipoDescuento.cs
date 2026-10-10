using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    // Forma en que se aplica el descuento de una linea de pedido.
    public enum TipoDescuento
    {
        [Display(Name = "Porcentaje")]
        Porcentaje = 1,

        [Display(Name = "Monto fijo")]
        Monto = 2
    }
}
