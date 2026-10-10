using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    // Un pedido se guarda solo al confirmarse.
    public enum EstadoPedido
    {
        [Display(Name = "Confirmado")]
        Confirmado = 1,

        [Display(Name = "Anulado")]
        Anulado = 2
    }
}
