namespace Pedidos360.ViewModels
{
    // Datos de paginacion sin el tipo de los elementos; los usa el partial _Paginacion.
    public interface IListaPaginada
    {
        int PaginaActual { get; }
        int TamanoPagina { get; }
        int TotalRegistros { get; }
        int TotalPaginas { get; }
        bool TienePaginaAnterior { get; }
        bool TienePaginaSiguiente { get; }
    }
}
