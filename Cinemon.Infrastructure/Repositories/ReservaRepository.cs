using Cinemon.Application.Abstractions;
using Cinemon.Application.Reservas.Queries.ObtenerReservas;
using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Entidades.Reservas;
using Cinemon.Domain.Exceptions;
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

        public async Task<bool> ButacasDisponiblesAsync(int funcionId, IReadOnlyCollection<int> butacasIds, CancellationToken cancellationToken)
        {
            return !await _context.ReservasButacas
                .AsNoTracking()
                .AnyAsync(
                    x =>
                        x.FuncionId == funcionId &&
                        butacasIds.Contains(x.ButacaId),
                    cancellationToken);
        }

        public async Task AddAsync(Reserva reserva, IReadOnlyCollection<int> butacasIds, CancellationToken cancellationToken)
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

                await _context.ReservasButacas.AddRangeAsync(reservaButacas,cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            } catch (DbUpdateException) {
                await transaction.RollbackAsync(cancellationToken);

                throw new ConflictException(
                    "Una o más butacas ya están reservadas para esta función.");
            } catch {
                await transaction.RollbackAsync(cancellationToken);
            }
        }
        public async Task<IReadOnlyCollection<Reserva>> ObtenerTodasAsync(CancellationToken cancellationToken)
        {
            return await _context.Reservas
                .AsNoTracking()
                .OrderByDescending(x => x.FechaReserva)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Reserva>> ObtenerPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken)
        {
            return await _context.Reservas
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId)
                .OrderByDescending(x => x.FechaReserva)
                .ToListAsync(cancellationToken);
        }

        public async Task<Reserva?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Reservas
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<bool> CancelarAsync(int id, CancellationToken cancellationToken)
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

        public async Task<IReadOnlyCollection<int>> ObtenerButacaIdsAsync(int reservaId, CancellationToken cancellationToken)
        {
            return await _context.ReservasButacas
                .AsNoTracking()
                .Where(x => x.ReservaId == reservaId)
                .Select(x => x.ButacaId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<ReservaDto>> ObtenerReservasAsync(int? usuarioId, CancellationToken cancellationToken)
        {
            var encabezados = await (
                from r in _context.Reservas.AsNoTracking()
                join u in _context.Usuarios.AsNoTracking() on r.UsuarioId equals u.Id
                join f in _context.Funciones.AsNoTracking() on r.FuncionId equals f.Id
                join p in _context.Peliculas.AsNoTracking() on f.PeliculaId equals p.Id
                join s in _context.Salas.AsNoTracking() on f.SalaId equals s.Id
                where usuarioId == null || r.UsuarioId == usuarioId
                orderby r.FechaReserva descending
                select new { Reserva = r, Usuario = u, Funcion = f, Pelicula = p, Sala = s })
                .ToListAsync(cancellationToken);

            if (encabezados.Count == 0)
                return Array.Empty<ReservaDto>();

            var reservaIds = encabezados.Select(x => x.Reserva.Id).ToList();

            var butacasPorReserva = await (
                from rb in _context.ReservasButacas.AsNoTracking()
                join b in _context.Butacas.AsNoTracking() on rb.ButacaId equals b.Id
                where reservaIds.Contains(rb.ReservaId)
                select new { rb.ReservaId, Butaca = b })
                .ToListAsync(cancellationToken);

            var butacasLookup = butacasPorReserva
                .GroupBy(x => x.ReservaId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyCollection<ButacaDto>)g
                        .Select(x => new ButacaDto(x.Butaca.Id, x.Butaca.Codigo, x.Butaca.Fila, x.Butaca.Numero))
                        .ToList());

            return encabezados
                .Select(x => new ReservaDto(
                    x.Reserva.Id,
                    new ClienteDto(x.Usuario.NombreApellido, x.Usuario.Email),
                    new FuncionDto(x.Funcion.Id, x.Funcion.FechaHoraInicio, x.Funcion.Idioma.ToString(), x.Funcion.Formato.ToString()),
                    new PeliculaDto(x.Pelicula.Titulo, x.Pelicula.Duracion, x.Pelicula.ClasificacionEdad.ToString(), x.Pelicula.PosterUrl),
                    new SalaDto(x.Sala.Numero, x.Sala.TipoSala.ToString()),
                    x.Reserva.FechaReserva,
                    x.Reserva.EstadoReserva.ToString(),
                    butacasLookup.TryGetValue(x.Reserva.Id, out var butacas) ? butacas : Array.Empty<ButacaDto>(),
                    x.Reserva.Total))
                .ToList();
        }

        public async Task<ReservaDto?> ObtenerDetalleAsync(int id, CancellationToken cancellationToken)
        {
            var encabezado = await (
                from r in _context.Reservas.AsNoTracking()
                join u in _context.Usuarios.AsNoTracking() on r.UsuarioId equals u.Id
                join f in _context.Funciones.AsNoTracking() on r.FuncionId equals f.Id
                join p in _context.Peliculas.AsNoTracking() on f.PeliculaId equals p.Id
                join s in _context.Salas.AsNoTracking() on f.SalaId equals s.Id
                where r.Id == id
                select new { Reserva = r, Usuario = u, Funcion = f, Pelicula = p, Sala = s })
                .FirstOrDefaultAsync(cancellationToken);

            if (encabezado is null)
                return null;

            var butacas = await (
                from rb in _context.ReservasButacas.AsNoTracking()
                join b in _context.Butacas.AsNoTracking() on rb.ButacaId equals b.Id
                where rb.ReservaId == id
                select new ButacaDto(b.Id, b.Codigo, b.Fila, b.Numero))
                .ToListAsync(cancellationToken);

            return new ReservaDto(
                encabezado.Reserva.Id,
                new ClienteDto(encabezado.Usuario.NombreApellido, encabezado.Usuario.Email),
                new FuncionDto(encabezado.Funcion.Id, encabezado.Funcion.FechaHoraInicio, encabezado.Funcion.Idioma.ToString(), encabezado.Funcion.Formato.ToString()),
                new PeliculaDto(encabezado.Pelicula.Titulo, encabezado.Pelicula.Duracion, encabezado.Pelicula.ClasificacionEdad.ToString(), encabezado.Pelicula.PosterUrl),
                new SalaDto(encabezado.Sala.Numero, encabezado.Sala.TipoSala.ToString()),
                encabezado.Reserva.FechaReserva,
                encabezado.Reserva.EstadoReserva.ToString(),
                butacas,
                encabezado.Reserva.Total);
        }
    }
}
