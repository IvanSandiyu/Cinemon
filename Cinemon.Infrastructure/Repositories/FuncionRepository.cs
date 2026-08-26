using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Funcion;
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
    }
}
