using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Butacas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class ButacaRepository : IButacaRepository
    {
        private readonly CinemonDbContext _context;

        public ButacaRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Butaca>> ObtenerPorSalaAsync(
            int salaId,
            CancellationToken cancellationToken)
        {
            return await _context.Butacas
                .AsNoTracking()
                .Where(x => x.SalaId == salaId)
                .OrderBy(x => x.Fila)
                .ThenBy(x => x.Numero)
                .ToListAsync(cancellationToken);
        }
    }
}
