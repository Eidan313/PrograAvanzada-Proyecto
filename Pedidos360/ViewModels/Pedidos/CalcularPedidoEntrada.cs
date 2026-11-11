using System.ComponentModel.DataAnnotations;
using Pedidos360.Models;

namespace Pedidos360.ViewModels.Pedidos;

// Una linea del pedido, el precio y el impuesto no se envian, los toma el servidor
public class LineaCalculoEntrada
{
    [Range(1, int.MaxValue, ErrorMessage = "El producto es requerido.")]
    public int ProductoId { get; set; }

    [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100.")]
    public int Cantidad { get; set; }

    public TipoDescuento TipoDescuento { get; set; } = TipoDescuento.Porcentaje;

    [Range(typeof(decimal), "0", "9999999999999999.99", ParseLimitsInInvariantCulture = true, ErrorMessage = "El descuento no puede ser negativo.")]
    public decimal Descuento { get; set; }
}

public class CalcularPedidoEntrada
{
    [Required, MinLength(1, ErrorMessage = "El pedido necesita al menos un producto."), MaxLength(30, ErrorMessage = "El pedido admite maximo 30 productos.")]
    public List<LineaCalculoEntrada> Lineas { get; set; } = [];
}
