namespace Pedidos360.ViewModels
{
    public class ClienteIndexViewModel
    {
        public IReadOnlyList<Models.Cliente> Clientes { get; set; } = Array.Empty<Models.Cliente>();
        public string? Busqueda { get; set; }
        public int Pagina { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalClientes { get; set; }
    }
}
