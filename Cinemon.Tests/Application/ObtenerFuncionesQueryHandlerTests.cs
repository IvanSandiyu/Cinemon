using Cinemon.Application.Common;
using Cinemon.Application.Funciones.Queries.ObtenerFunciones;
using Cinemon.Domain.Enums;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Infrastructure;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    public class ObtenerFuncionesQueryHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private ObtenerFuncionesQueryHandler CrearHandler() =>
            new(
                new FuncionRepository(_db.Context),
                new PeliculaRepository(_db.Context),
                TimeProvider.System);

        private async Task<EstadoFuncion> ObtenerEstado(int funcionId)
        {
            var funciones = await CrearHandler().Handle(
                new ObtenerFuncionesQuery(),
                CancellationToken.None);

            return funciones.Single(x => x.Id == funcionId).EstadoFuncion;
        }

        [Fact]
        public async Task MarcaComoFinalizadaUnaFuncionQueYaTermino()
        {
            var pelicula = await _db.CrearPeliculaAsync(duracion: 120);
            var sala = await _db.CrearSalaAsync();

            var yaTermino = TimeProvider.System.Ahora().AddHours(-3);

            var funcion = await _db.CrearFuncionAsync(
                pelicula.Id,
                sala.Id,
                yaTermino);

            Assert.Equal(EstadoFuncion.Finalizada, await ObtenerEstado(funcion.Id));
        }

        [Fact]
        public async Task DejaProgramadaUnaFuncionQueTodaviaNoTermino()
        {
            var pelicula = await _db.CrearPeliculaAsync(duracion: 120);
            var sala = await _db.CrearSalaAsync();

            var empezo = TimeProvider.System.Ahora().AddMinutes(-30);

            var funcion = await _db.CrearFuncionAsync(
                pelicula.Id,
                sala.Id,
                empezo);

            Assert.Equal(EstadoFuncion.Programada, await ObtenerEstado(funcion.Id));
        }

        [Fact]
        public async Task DejaProgramadaUnaFuncionFutura()
        {
            var pelicula = await _db.CrearPeliculaAsync(duracion: 120);
            var sala = await _db.CrearSalaAsync();

            var funcion = await _db.CrearFuncionAsync(
                pelicula.Id,
                sala.Id,
                TimeProvider.System.Ahora().AddDays(1));

            Assert.Equal(EstadoFuncion.Programada, await ObtenerEstado(funcion.Id));
        }

        [Fact]
        public async Task RespetaElEstadoCancelada()
        {
            var pelicula = await _db.CrearPeliculaAsync(duracion: 120);
            var sala = await _db.CrearSalaAsync();

            var funcion = await _db.CrearFuncionAsync(
                pelicula.Id,
                sala.Id,
                TimeProvider.System.Ahora().AddHours(-3),
                estado: EstadoFuncion.Cancelada);

            Assert.Equal(EstadoFuncion.Cancelada, await ObtenerEstado(funcion.Id));
        }

        [Fact]
        public async Task RespetaElEstadoFinalizadaPersistida()
        {
            var pelicula = await _db.CrearPeliculaAsync(duracion: 120);
            var sala = await _db.CrearSalaAsync();

            var funcion = await _db.CrearFuncionAsync(
                pelicula.Id,
                sala.Id,
                TimeProvider.System.Ahora().AddHours(-3),
                estado: EstadoFuncion.Finalizada);

            Assert.Equal(EstadoFuncion.Finalizada, await ObtenerEstado(funcion.Id));
        }
    }
}
