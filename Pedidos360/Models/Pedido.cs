using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Pedidos360.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El cliente es requerido.")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        public Cliente? Cliente { get; set; }

        // Usuario de Identity que confirmo el pedido.
        [Required]
        [StringLength(450)]
        public string UsuarioId { get; set; } = string.Empty;

        public IdentityUser? Usuario { get; set; }

        // Fecha de confirmacion
        public DateTime Fecha { get; set; }

        public EstadoPedido Estado { get; set; } = EstadoPedido.Confirmado;

        // Totales guardados para auditoria (enunciado, seccion 4). Total = Subtotal + Impuestos.
        public decimal Subtotal { get; set; }

        public decimal Impuestos { get; set; }

        public decimal Total { get; set; }

        public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    }
}
