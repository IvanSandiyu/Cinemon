using Cinemon.Application.Common;
using Cinemon.Application.Funciones.Commands.CancelarFuncion;
using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    public class CancelarFuncionCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private CancelarFuncionCommandHandler CrearHandler() =>
            new(new FuncionRepository(_db.Context));

        private async Task<Funcion> CrearFuncion(
            int peliculaId,
            int salaId,
            DateTime fechaHoraInicio,
            EstadoFuncion estado = EstadoFuncion.Programada)
        {
            var funcion = await _db.CrearFuncionAsync(
                peliculaId,
                salaId,
                fechaHoraInicio,
                estado: estado);

            _db.Context.Entry(funcion).State = EntityState.Detached;

            return funcion;
        }

        [Fact]
        public async Task CancelaUnaFuncionProgramada()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var funcion = await CrearFuncion(
                pelicula.Id,
                sala.Id,
                TimeProvider.System.Ahora().AddDays(1));

            await CrearHandler().Handle(
                new CancelarFuncionCommand(funcion.Id),
                CancellationToken.None);

            var actualizada = await _db.Context.Funciones
                .AsNoTracking()
                .FirstAsync(x => x.Id == funcion.Id);

            Assert.Equal(EstadoFuncion.Cancelada, actualizada.EstadoFuncion);
        }

        [Fact]
        public async Task FallaSiLaFuncionNoExiste()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler().Handle(
                    new CancelarFuncionCommand(9999),
                    CancellationToken.None));
        }

        [Theory]
        [InlineData(EstadoFuncion.Finalizada)]
        [InlineData(EstadoFuncion.Cancelada)]
        public async Task FallaSiLaFuncionNoEstaProgramada(EstadoFuncion estado)
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var funcion = await CrearFuncion(
                pelicula.Id,
                sala.Id,
                TimeProvider.System.Ahora().AddHours(-1),
                estado);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    new CancelarFuncionCommand(funcion.Id),
                    CancellationToken.None));
        }
    }
}
