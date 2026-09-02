using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Enums;
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

        public async Task<IReadOnlyCollection<Butaca>> ObtenerPorIdsAsync(IReadOnlyCollection<int> ids,CancellationToken cancellationToken)
        {
            return await _context.Butacas
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(cancellationToken);
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

        public async Task<IReadOnlyCollection<Butaca>> ObtenerPorFuncionAsync(int funcionId,CancellationToken cancellationToken)
        {
            var funcion = await _context.Funciones
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == funcionId,
                    cancellationToken);

            if (funcion is null)
                return [];

            return await _context.Butacas
                .AsNoTracking()
                .Where(x => x.SalaId == funcion.SalaId)
                .OrderBy(x => x.Fila)
                .ThenBy(x => x.Numero)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<int>> ObtenerIdsOcupadosPorFuncionAsync(int funcionId,CancellationToken cancellationToken)
        {
            return await _context.ReservasButacas
                .AsNoTracking()
                .Where(x => x.FuncionId == funcionId)
                .Select(x => x.ButacaId)
                .ToListAsync(cancellationToken);
        }
    }
}
