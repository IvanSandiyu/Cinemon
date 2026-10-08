using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Candy;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class ProductoCandyRepository : IProductoCandyRepository
    {
        private readonly CinemonDbContext _context;

        public ProductoCandyRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<ProductoCandy>> ObtenerTodasAsync(
            CancellationToken cancellationToken)
        {
            return await _context.ProductosCandy
                .AsNoTracking()
                .Include(x => x.Componentes)
                .ThenInclude(x => x.Componente)
                .OrderBy(x => x.Categoria)
                .ThenBy(x => x.Nombre)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<ProductoCandy>> ObtenerActivasAsync(
            CancellationToken cancellationToken)
        {
            return await _context.ProductosCandy
                .AsNoTracking()
                .Where(x => x.Activo)
                .Include(x => x.Componentes)
                .ThenInclude(x => x.Componente)
                .OrderBy(x => x.Categoria)
                .ThenBy(x => x.Nombre)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductoCandy?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.ProductosCandy
                .Include(x => x.Componentes)
                .ThenInclude(x => x.Componente)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<ProductoCandy>> ObtenerPorIdsAsync(
            IReadOnlyCollection<int> ids,
            CancellationToken cancellationToken)
        {
            if (ids is null || ids.Count == 0)
                return [];

            return await _context.ProductosCandy
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Include(x => x.Componentes)
                .ThenInclude(x => x.Componente)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(
            ProductoCandy producto,
            CancellationToken cancellationToken)
        {
            await _context.ProductosCandy.AddAsync(producto, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(ProductoCandy producto)
        {
            _context.ProductosCandy.Update(producto);
        }

        public async Task<bool> EsComponenteDeAlgunComboAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.ProductoComboItems
                .AnyAsync(x => x.ComponenteProductoId == id, cancellationToken);
        }

        public async Task<bool> TienePedidosAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.PedidosCandyItems
                .AnyAsync(x => x.ProductoId == id, cancellationToken);
        }

        public async Task EliminarAsync(
            int id,
            CancellationToken cancellationToken)
        {
            var producto = await _context.ProductosCandy
                .Include(x => x.Componentes)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (producto is null)
                return;

            _context.ProductosCandy.Remove(producto);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task GuardarAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
