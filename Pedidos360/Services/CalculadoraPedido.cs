using Pedidos360.Models;
using Pedidos360.ViewModels.Pedidos;

namespace Pedidos360.Services;

// Formulas del pedido
public static class CalculadoraPedido
{
    public static LineaCalculadaDto CalcularLinea(int cantidad, decimal precio, decimal impuestoPorc, TipoDescuento tipo, decimal descuento)
    {
        decimal bruto = cantidad * precio;

        decimal montoDescuento = tipo switch
        {
            TipoDescuento.Porcentaje when descuento >= 0 && descuento <= 100 => bruto * descuento / 100,
            TipoDescuento.Porcentaje => throw new ReglaNegocioException(422, "El descuento en porcentaje debe estar entre 0 y 100."),
            TipoDescuento.Monto when descuento >= 0 && descuento <= bruto => descuento,
            TipoDescuento.Monto => throw new ReglaNegocioException(422, "El descuento en monto no puede ser mayor al valor de la linea."),
            _ => throw new ReglaNegocioException(422, "El tipo de descuento no es valido.")
        };

        // Se redondea cada linea, el total del pedido siempre es subtotal + impuestos
        montoDescuento = Math.Round(montoDescuento, 2, MidpointRounding.AwayFromZero);
        decimal subtotal = bruto - montoDescuento;
        decimal impuesto = Math.Round(subtotal * impuestoPorc / 100, 2, MidpointRounding.AwayFromZero);

        return new LineaCalculadaDto
        {
            Cantidad = cantidad,
            PrecioUnit = precio,
            TipoDescuento = tipo,
            Descuento = descuento,
            DescuentoMonto = montoDescuento,
            ImpuestoPorc = impuestoPorc,
            Subtotal = subtotal,
            Impuesto = impuesto,
            TotalLinea = subtotal + impuesto
        };
    }

    public static ResultadoCalculoDto CalcularTotales(List<LineaCalculadaDto> lineas)
    {
        decimal subtotal = lineas.Sum(l => l.Subtotal);
        decimal impuestos = lineas.Sum(l => l.Impuesto);
        return new ResultadoCalculoDto { Lineas = lineas, Subtotal = subtotal, Impuestos = impuestos, Total = subtotal + impuestos };
    }
}
