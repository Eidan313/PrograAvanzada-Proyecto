using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedidos360.Data;
using Pedidos360.Models;
using Pedidos360.Services.Interfaces;
using Pedidos360.ViewModels;

namespace Pedidos360.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ApplicationDbContext _context;

        public CategoriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<ListaPaginada<CategoriaFilaViewModel>> ObtenerPaginaAsync(int pagina)
        {
            var consulta = _context.Categorias
                .AsNoTracking()
                .OrderBy(categoria => categoria.Nombre)
                .ThenBy(categoria => categoria.Id)
                .Select(categoria => new CategoriaFilaViewModel
                {
                    Id = categoria.Id,
                    Nombre = categoria.Nombre,
                    CantidadProductos = categoria.Productos.Count
                });

            return ListaPaginada<CategoriaFilaViewModel>.CrearAsync(consulta, pagina);
        }

        public Task<Categoria?> ObtenerPorIdAsync(int id)
        {
            return _context.Categorias.AsNoTracking().FirstOrDefaultAsync(categoria => categoria.Id == id);
        }

        // La comparacion no distingue mayusculas (collation por defecto de SQL Server).
        public Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
        {
            var normalizado = nombre.Trim();
            var consulta = _context.Categorias.AsNoTracking().Where(categoria => categoria.Nombre == normalizado);

            if (idExcluir.HasValue)
            {
                consulta = consulta.Where(categoria => categoria.Id != idExcluir.Value);
            }

            return consulta.AnyAsync();
        }

        public async Task<bool> CrearAsync(Categoria categoria)
        {
            categoria.Nombre = categoria.Nombre.Trim();
            if (await ExisteNombreAsync(categoria.Nombre))
            {
                return false;
            }

            _context.Categorias.Add(categoria);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException exception) when (EsNombreDuplicado(exception))
            {
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(Categoria categoria)
        {
            var existente = await _context.Categorias.FirstOrDefaultAsync(actual => actual.Id == categoria.Id);
            if (existente is null || await ExisteNombreAsync(categoria.Nombre, categoria.Id))
            {
                return false;
            }

            existente.Nombre = categoria.Nombre.Trim();

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException exception) when (EsNombreDuplicado(exception))
            {
                return false;
            }
        }

        public Task<int> ContarProductosAsync(int id)
        {
            return _context.Productos.AsNoTracking().CountAsync(producto => producto.CategoriaId == id);
        }

        public async Task<bool> EliminarSiNoTieneProductosAsync(int id)
        {
            var categoria = await _context.Categorias.FirstOrDefaultAsync(actual => actual.Id == id);
            if (categoria is null || await ContarProductosAsync(id) > 0)
            {
                return false;
            }

            _context.Categorias.Remove(categoria);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException exception) when (EsRestriccionDeRelacion(exception))
            {
                // Alguien agrego un producto justo antes de borrar: la FK (Restrict) lo impide.
                return false;
            }
        }

        // 2601 y 2627: indice unico violado.
        private static bool EsNombreDuplicado(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException &&
                   (sqlException.Number == 2601 || sqlException.Number == 2627);
        }

        // 547: conflicto con llave foranea.
        private static bool EsRestriccionDeRelacion(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sqlException && sqlException.Number == 547;
        }
    }
}
