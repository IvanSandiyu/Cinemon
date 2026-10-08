using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Candy;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class PedidoCandyRepository : IPedidoCandyRepository
    {
        private readonly CinemonDbContext _context;

        public PedidoCandyRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(
            PedidoCandy pedido,
            CancellationToken cancellationToken)
        {
            await _context.PedidosCandy.AddAsync(pedido, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<PedidoCandy>> ObtenerPorUsuarioAsync(
            int usuarioId,
            CancellationToken cancellationToken)
        {
            return await _context.PedidosCandy
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId)
                .Include(x => x.Items)
                .OrderByDescending(x => x.FechaPedido)
                .ToListAsync(cancellationToken);
        }
    }
}
