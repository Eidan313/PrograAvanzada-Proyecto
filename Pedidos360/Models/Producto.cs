using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El código es requerido.")]
        [StringLength(20, ErrorMessage = "El código no puede superar los 20 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La categoría es requerida.")]
        [Display(Name = "Categoría")]
        public int CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }

        [Range(typeof(decimal), "0.01", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "El precio debe ser mayor a 0.")]
        public decimal Precio { get; set; }

        [Range(typeof(decimal), "0", "100", ParseLimitsInInvariantCulture = true, ErrorMessage = "El impuesto debe estar entre 0 y 100.")]
        [Display(Name = "Impuesto (%)")]
        public decimal ImpuestoPorc { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int Stock { get; set; }

        // La imagen se guarda en la base (varbinary(max)), no url
        [Required(ErrorMessage = "La imagen es requerida.")]
        public byte[] Imagen { get; set; } = [];

        // Tipo de archivo de la imagen: "image/jpeg" o "image/png"
        [Required]
        [StringLength(30)]
        public string ImagenTipo { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        public ICollection<PedidoDetalle> Detalles { get; set; } = new List<PedidoDetalle>();
    }
}
