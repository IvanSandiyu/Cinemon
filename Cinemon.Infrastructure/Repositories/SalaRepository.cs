using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Salas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class SalaRepository : ISalaRepository
    {
        private readonly CinemonDbContext _context;

        public SalaRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Sala>> ObtenerTodasAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Salas
                .AsNoTracking()
                .Include(x => x.Butacas)
                .OrderBy(x => x.Numero)
                .ToListAsync(cancellationToken);
        }
        public async Task<Sala?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken)
        {
            return await _context.Salas
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }
    }
}
