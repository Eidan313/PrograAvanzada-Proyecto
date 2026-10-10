using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    public class PedidoDetalle
    {
        public int Id { get; set; }

        public int PedidoId { get; set; }

        public Pedido? Pedido { get; set; }

        [Required(ErrorMessage = "El producto es requerido.")]
        [Display(Name = "Producto")]
        public int ProductoId { get; set; }

        public Producto? Producto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }

        // Precio al momento de la venta (valor histrico, no el precio actual del producto)
        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Display(Name = "Precio unitario")]
        public decimal PrecioUnit { get; set; }

        [Display(Name = "Tipo de descuento")]
        public TipoDescuento TipoDescuento { get; set; } = TipoDescuento.Porcentaje;

        // Si es porcentaje: entre 0 y 100. Si es monto: no mayor a Cantidad x PrecioUnit.
        [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "El descuento no puede ser negativo.")]
        public decimal Descuento { get; set; }

        // Impuesto al momento de la venta (valor histórico)
        [Range(typeof(decimal), "0", "100", ParseLimitsInInvariantCulture = true, ErrorMessage = "El impuesto debe estar entre 0 y 100.")]
        [Display(Name = "Impuesto (%)")]
        public decimal ImpuestoPorc { get; set; }

        // Total guardado para auditoria: (Cantidad x PrecioUnit - descuento) + impuesto
        [Display(Name = "Total de linea")]
        public decimal TotalLinea { get; set; }
    }
}
