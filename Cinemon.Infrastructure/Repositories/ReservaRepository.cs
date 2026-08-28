using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Entidades.Reservas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class ReservaRepository : IReservaRepository
    {
        private readonly CinemonDbContext _context;

        public ReservaRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ButacasDisponiblesAsync(
            int funcionId,
            IReadOnlyCollection<int> butacasIds,
            CancellationToken cancellationToken)
        {
            return !await _context.ReservasButacas
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.FuncionId == funcionId &&
                        butacasIds.Contains(x.ButacaId),
                    cancellationToken);
        }

        public async Task AddAsync(
            Reserva reserva,
            IReadOnlyCollection<int> butacasIds,
            CancellationToken cancellationToken)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            try {
                // Guardamos la reserva primero para obtener su Id
                await _context.Reservas.AddAsync(
                    reserva,
                    cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                // Ahora reserva.Id ya fue generado por SQL Server
                var reservaButacas = butacasIds
                    .Select(
                        butacaId => new ReservaButaca(
                            reserva.Id,
                            reserva.FuncionId,
                            butacaId))
                    .ToList();

                await _context.ReservasButacas.AddRangeAsync(
                    reservaButacas,
                    cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);
            } catch {
                await transaction.RollbackAsync(
                    cancellationToken);

                throw;
            }
        }
        public async Task<IReadOnlyCollection<Reserva>> ObtenerTodasAsync(
            CancellationToken cancellationToken)
        {
            return await _context.Reservas
                .AsNoTracking()
                .OrderByDescending(x => x.FechaReserva)
                .ToListAsync(cancellationToken);
        }

        public async Task<Reserva?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken)
        {
            return await _context.Reservas
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<bool> CancelarAsync(
            int id,
            CancellationToken cancellationToken)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    cancellationToken);

            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);

            if (reserva is null)
                return false;

            reserva.Cancelar();

            var reservasButacas = await _context.ReservasButacas
                .Where(x => x.ReservaId == id)
                .ToListAsync(cancellationToken);

            _context.ReservasButacas.RemoveRange(reservasButacas);

            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return true;
        }

        public async Task<IReadOnlyCollection<int>> ObtenerButacaIdsAsync(
            int reservaId,
            CancellationToken cancellationToken)
        {
            return await _context.ReservasButacas
                .AsNoTracking()
                .Where(x => x.ReservaId == reservaId)
                .Select(x => x.ButacaId)
                .ToListAsync(cancellationToken);
        }
    }
}
