using Cinemon.Application.Common;
using Cinemon.Application.Funciones.Commands.FinalizarFuncion;
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
    public class FinalizarFuncionCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private FinalizarFuncionCommandHandler CrearHandler() =>
            new(
                new FuncionRepository(_db.Context),
                TimeProvider.System);

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
        public async Task FinalizaUnaFuncionProgramada()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var yaComenzo = TimeProvider.System.Ahora().AddHours(-1);

            var funcion = await CrearFuncion(
                pelicula.Id,
                sala.Id,
                yaComenzo);

            await CrearHandler().Handle(
                new FinalizarFuncionCommand(funcion.Id),
                CancellationToken.None);

            var actualizada = await _db.Context.Funciones
                .AsNoTracking()
                .FirstAsync(x => x.Id == funcion.Id);

            Assert.Equal(EstadoFuncion.Finalizada, actualizada.EstadoFuncion);
        }

        [Fact]
        public async Task FallaSiLaFuncionNoExiste()
        {
            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler().Handle(
                    new FinalizarFuncionCommand(9999),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiLaFuncionTodaviaNoComenzo()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var futura = TimeProvider.System.Ahora().AddHours(1);

            var funcion = await CrearFuncion(
                pelicula.Id,
                sala.Id,
                futura);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    new FinalizarFuncionCommand(funcion.Id),
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
                    new FinalizarFuncionCommand(funcion.Id),
                    CancellationToken.None));
        }
    }
}
