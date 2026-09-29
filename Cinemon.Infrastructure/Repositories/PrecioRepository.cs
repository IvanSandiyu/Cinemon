using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Precios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class PrecioRepository : IPrecioRepository
    {
        private readonly CinemonDbContext _context;

        public PrecioRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Precio>> ObtenerTodosAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Precios
                .AsNoTracking()
                .OrderBy(x => x.TipoSala)
                .ThenBy(x => x.Formato)
                .ToListAsync(cancellationToken);
        }

        public async Task<Precio?> ObtenerPorIdAsync(int id,
            CancellationToken cancellationToken)
        {
            return await _context.Precios
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task GuardarAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
