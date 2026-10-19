using Cinemon.Application.Common;
using Cinemon.Application.Funciones.FinalizarFuncionVencida;
using Cinemon.Domain.Enums;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    public class FinalizarFuncionesVencidasCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private FinalizarFuncionesVencidasCommandHandler CrearHandler() =>
            new(
                new FuncionRepository(_db.Context),
                TimeProvider.System);

        private Task<EstadoFuncion> EstadoEnBase(int funcionId) =>
            _db.Context.Funciones
                .AsNoTracking()
                .Where(x => x.Id == funcionId)
                .Select(x => x.EstadoFuncion)
                .FirstAsync();

        [Fact]
        public async Task FinalizaSoloLasFuncionesQueYaTerminaron()
        {
            var pelicula = await _db.CrearPeliculaAsync(duracion: 120);
            var sala = await _db.CrearSalaAsync();

            var termino = await _db.CrearFuncionAsync(
                pelicula.Id, sala.Id, TimeProvider.System.Ahora().AddHours(-4));

            var enCurso = await _db.CrearFuncionAsync(
                pelicula.Id, sala.Id, TimeProvider.System.Ahora().AddMinutes(-30));

            var futura = await _db.CrearFuncionAsync(
                pelicula.Id, sala.Id, TimeProvider.System.Ahora().AddDays(1));

            var cantidad = await CrearHandler().Handle(
                new FinalizarFuncionesVencidasCommand(),
                CancellationToken.None);

            Assert.Equal(1, cantidad);
            Assert.Equal(EstadoFuncion.Finalizada, await EstadoEnBase(termino.Id));
            Assert.Equal(EstadoFuncion.Programada, await EstadoEnBase(enCurso.Id));
            Assert.Equal(EstadoFuncion.Programada, await EstadoEnBase(futura.Id));
        }

        [Fact]
        public async Task NoHaceNadaSiNoHayFuncionesVencidas()
        {
            var pelicula = await _db.CrearPeliculaAsync(duracion: 120);
            var sala = await _db.CrearSalaAsync();

            await _db.CrearFuncionAsync(
                pelicula.Id, sala.Id, TimeProvider.System.Ahora().AddDays(1));

            var cantidad = await CrearHandler().Handle(
                new FinalizarFuncionesVencidasCommand(),
                CancellationToken.None);

            Assert.Equal(0, cantidad);
        }

        [Fact]
        public async Task NoVuelveAFinalizarLasQueYaEstanFinalizadas()
        {
            var pelicula = await _db.CrearPeliculaAsync(duracion: 120);
            var sala = await _db.CrearSalaAsync();

            await _db.CrearFuncionAsync(
                pelicula.Id, sala.Id, TimeProvider.System.Ahora().AddHours(-4),
                estado: EstadoFuncion.Finalizada);

            var cantidad = await CrearHandler().Handle(
                new FinalizarFuncionesVencidasCommand(),
                CancellationToken.None);

            Assert.Equal(0, cantidad);
        }
    }
}
