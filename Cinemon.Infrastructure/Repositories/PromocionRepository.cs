using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Promociones;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class PromocionRepository : IPromocionRepository
    {
        private readonly CinemonDbContext _context;

        public PromocionRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Promocion>> ObtenerTodasAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Promociones
                .AsNoTracking()
                .Include(x => x.DiasSemana)
                .OrderBy(x => x.Nombre)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Promocion>> ObtenerActivasParaFechaAsync(
            DateTime fechaHoraFuncion,
            CancellationToken cancellationToken)
        {
            var fecha = fechaHoraFuncion.Date;

            return await _context.Promociones
                .AsNoTracking()
                .Include(x => x.DiasSemana)
                .Where(x =>
                    x.Activa &&
                    x.FechaDesde <= fecha &&
                    x.FechaHasta >= fecha &&
                    x.DiasSemana.Any(d => d.Dia == fechaHoraFuncion.DayOfWeek))
                .ToListAsync(cancellationToken);
        }

        public async Task<Promocion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Promociones
                .Include(x => x.DiasSemana)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task AddAsync(Promocion promocion, CancellationToken cancellationToken)
        {
            await _context.Promociones.AddAsync(promocion, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Update(Promocion promocion)
        {
            _context.Promociones.Update(promocion);
        }

        public async Task EliminarAsync(int id, CancellationToken cancellationToken)
        {
            var promocion = await ObtenerPorIdAsync(id, cancellationToken);

            if (promocion is null)
                return;

            _context.Promociones.Remove(promocion);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task GuardarAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
