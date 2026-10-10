using System.ComponentModel.DataAnnotations;

namespace Pedidos360.Models
{
    // Tipo de identificacion del cliente. Se guarda como entero en la tabla Clientes.
    public enum TipoIdentificacion
    {
        [Display(Name = "Cédula física")]
        Fisica = 1,

        [Display(Name = "Cédula jurídica")]
        Juridica = 2
    }
}
