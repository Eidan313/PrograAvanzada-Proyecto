using Microsoft.EntityFrameworkCore;

namespace Pedidos360.ViewModels
{
    // Pagina de resultados que usan todos los listados (Productos, Clientes, Categorias).
    public class ListaPaginada<T> : IListaPaginada
    {
        public const int TamanoPorDefecto = 10;

        public IReadOnlyList<T> Elementos { get; }
        public int PaginaActual { get; }
        public int TamanoPagina { get; }
        public int TotalRegistros { get; }

        public int TotalPaginas => (int)Math.Ceiling(TotalRegistros / (double)TamanoPagina);
        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

        public ListaPaginada(IReadOnlyList<T> elementos, int totalRegistros, int paginaActual, int tamanoPagina)
        {
            Elementos = elementos;
            TotalRegistros = totalRegistros;
            PaginaActual = paginaActual;
            TamanoPagina = tamanoPagina;
        }

        // Cuenta y trae solo los registros de la pagina pedida (Skip/Take se ejecutan en SQL).
        // La consulta debe venir ordenada para que las paginas sean estables.
        public static async Task<ListaPaginada<T>> CrearAsync(IQueryable<T> consulta, int pagina, int tamanoPagina = TamanoPorDefecto)
        {
            if (tamanoPagina < 1)
            {
                tamanoPagina = TamanoPorDefecto;
            }

            var totalRegistros = await consulta.CountAsync();
            var totalPaginas = Math.Max(1, (int)Math.Ceiling(totalRegistros / (double)tamanoPagina));

            // Si piden una pagina fuera de rango se muestra la primera o la ultima.
            pagina = Math.Clamp(pagina, 1, totalPaginas);

            var elementos = await consulta
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new ListaPaginada<T>(elementos, totalRegistros, pagina, tamanoPagina);
        }
    }
}
