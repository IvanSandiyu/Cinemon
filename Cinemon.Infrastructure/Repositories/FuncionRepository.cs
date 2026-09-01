using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Entidades.Peliculas;
using Cinemon.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Repositories
{
    public class FuncionRepository : IFuncionRepository
    {
        private readonly CinemonDbContext _context;

        public FuncionRepository(CinemonDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(
            Funcion funcion,
            CancellationToken cancellationToken)
        {
            await _context.Funciones.AddAsync(
                funcion,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);
        }

        public async Task<bool> ExisteSuperposicionAsync(int salaId,DateTime fechaHoraInicio,DateTime fechaHoraFin,CancellationToken cancellationToken)
        {
            return await (
                from funcion in _context.Funciones.AsNoTracking()
                join pelicula in _context.Peliculas.AsNoTracking()
                    on funcion.PeliculaId equals pelicula.Id
                where funcion.SalaId == salaId
                      && funcion.EstadoFuncion != EstadoFuncion.Cancelada
                let funcionFechaHoraFin =
                    funcion.FechaHoraInicio.AddMinutes(pelicula.Duracion)
                where funcion.FechaHoraInicio < fechaHoraFin
                      && fechaHoraInicio < funcionFechaHoraFin
                select funcion
            ).AnyAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<Funcion>> ObtenerFuncionesAsync(CancellationToken cancellationToken)
        {
            return await _context.Funciones.AsNoTracking().ToListAsync(cancellationToken);
        }

        public async Task<Funcion?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken)
        {
            return await _context.Funciones
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == id,
                    cancellationToken);
        }

        public async Task<bool> ExisteSuperposicionAsync(int salaId,DateTime fechaHoraInicio,DateTime fechaHoraFin,
            int? funcionId,
            CancellationToken cancellationToken)
        {
            return await _context.Funciones
                .AsNoTracking()
                .Where(x =>
                    x.SalaId == salaId &&
                    x.EstadoFuncion != EstadoFuncion.Cancelada &&
                    (!funcionId.HasValue || x.Id != funcionId.Value))
                .Join(
                    _context.Peliculas,
                    funcion => funcion.PeliculaId,
                    pelicula => pelicula.Id,
                    (funcion, pelicula) => new
                    {
                        funcion.Id,
                        funcion.FechaHoraInicio,
                        pelicula.Duracion
                    })
                .AnyAsync(
                    x =>
                        x.FechaHoraInicio < fechaHoraFin &&
                        fechaHoraInicio <
                            x.FechaHoraInicio.AddMinutes(x.Duracion),
                    cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            Funcion funcion,
            CancellationToken cancellationToken)
        {
            _context.Funciones.Update(funcion);

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Pelicula?> ObtenerPeliculaAsync(int peliculaId,CancellationToken cancellationToken)
        {
            return await _context.Peliculas
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == peliculaId,
                    cancellationToken);
        }
    }
}
