namespace Pedidos360.Models;

// Codigo HTTP que se devuelve (404, 409, 422)
public class ReglaNegocioException(int estado, string mensaje) : Exception(mensaje)
{
    public int Estado { get; } = estado;
}
