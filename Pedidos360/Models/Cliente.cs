using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El tipo de identificación es requerido.")]
        [Display(Name = "Tipo de identificación")]
        public TipoIdentificacion TipoIdentificacion { get; set; } = TipoIdentificacion.Fisica;

        // Solo digitos, 9 para fisica y 10 para juridica.
        [Required(ErrorMessage = "La cédula es requerida.")]
        [RegularExpression(@"^\d{9,10}$", ErrorMessage = "La cédula debe tener 9 o 10 dígitos, sin guiones.")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede superar los 100 caracteres.")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es requerido.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El teléfono debe tener 8 dígitos, sin guiones.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es requerida.")]
        [StringLength(250, ErrorMessage = "La dirección no puede superar los 250 caracteres.")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
