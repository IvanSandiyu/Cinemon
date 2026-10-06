using Cinemon.Application.Funciones.Commands.CrearFuncion;
using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    public class CrearFuncionCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private CrearFuncionCommandHandler CrearHandler() =>
            new(
                new PeliculaRepository(_db.Context),
                new SalaRepository(_db.Context),
                new FuncionRepository(_db.Context));

        private CrearFuncionCommand CrearComando(
            int peliculaId,
            int salaId,
            DateTime fechaHoraInicio,
            Formato formato = Formato.DosD) =>
            new(peliculaId,salaId,fechaHoraInicio,IdiomaFuncion.Espanol,formato,10000m);

        [Fact]
        public async Task CreaLaFuncionYDevuelveElIdGenerado()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var id = await CrearHandler().Handle(
                CrearComando(pelicula.Id,sala.Id,new DateTime(2024,5,6,20,0,0)),
                CancellationToken.None);

            Assert.True(id > 0);

            var funcion = await _db.Context.Funciones
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == id);

            Assert.NotNull(funcion);
            Assert.Equal(pelicula.Id,funcion!.PeliculaId);
            Assert.Equal(sala.Id,funcion.SalaId);
            Assert.Equal(10000m,funcion.Precio);
            Assert.Equal(EstadoFuncion.Programada,funcion.EstadoFuncion);
        }

        [Fact]
        public async Task ActivaLaPeliculaAlCrearSuPrimeraFuncion()
        {
            var pelicula = await _db.CrearPeliculaAsync(activa: false);
            var sala = await _db.CrearSalaAsync();

            await CrearHandler().Handle(
                CrearComando(pelicula.Id,sala.Id,new DateTime(2024,5,6,20,0,0)),
                CancellationToken.None);

            var actualizada = await _db.Context.Peliculas
                .AsNoTracking()
                .SingleAsync(x => x.Id == pelicula.Id);

            Assert.True(actualizada.Activa);
        }

        [Fact]
        public async Task FallaSiLaPeliculaNoExiste()
        {
            var sala = await _db.CrearSalaAsync();

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler().Handle(
                    CrearComando(9999,sala.Id,new DateTime(2024,5,6,20,0,0)),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiLaSalaNoExiste()
        {
            var pelicula = await _db.CrearPeliculaAsync();

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,9999,new DateTime(2024,5,6,20,0,0)),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiLaSalaEsImaxYLaFuncionEsTresD()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync(TipoSala.Imax);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,sala.Id,new DateTime(2024,5,6,20,0,0),Formato.TresD),
                    CancellationToken.None));

            Assert.Empty(await _db.Context.Funciones.ToListAsync());
        }

        [Fact]
        public async Task FallaSiLaSalaYaTieneUnaFuncionSuperpuesta()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            await _db.CrearFuncionAsync(pelicula.Id,sala.Id,new DateTime(2024,5,6,20,0,0));

            // Arranca 60 minutos después de la primera y dura 120.
            await Assert.ThrowsAsync<ConflictException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,sala.Id,new DateTime(2024,5,6,21,0,0)),
                    CancellationToken.None));

            Assert.Single(await _db.Context.Funciones.ToListAsync());
        }

        [Fact]
        public async Task FallaSiLaFuncionSeSuperponeConLaAnterior()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            await _db.CrearFuncionAsync(pelicula.Id,sala.Id,new DateTime(2024,5,6,20,0,0));

            await Assert.ThrowsAsync<ConflictException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,sala.Id,new DateTime(2024,5,6,21,30,0)),
                    CancellationToken.None));
        }

        [Fact]
        public async Task PermiteDosFuncionesConsecutivasEnLaMismaSala()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            await _db.CrearFuncionAsync(pelicula.Id,sala.Id,new DateTime(2024,5,6,20,0,0));

            // La primera termina 22:00, así que 22:00 no se superpone.
            await CrearHandler().Handle(
                CrearComando(pelicula.Id,sala.Id,new DateTime(2024,5,6,22,0,0)),
                CancellationToken.None);

            Assert.Equal(2,await _db.Context.Funciones.CountAsync());
        }

        [Fact]
        public async Task LaSuperposicionSeValidaSoloDentroDeLaMismaSala()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var salaA = await _db.CrearSalaAsync();
            var salaB = await _db.CrearSalaAsync();

            await _db.CrearFuncionAsync(pelicula.Id,salaA.Id,new DateTime(2024,5,6,20,0,0));

            await CrearHandler().Handle(
                CrearComando(pelicula.Id,salaB.Id,new DateTime(2024,5,6,20,30,0)),
                CancellationToken.None);

            Assert.Equal(2,await _db.Context.Funciones.CountAsync());
        }

        [Fact]
        public async Task NoCreaLaFuncionSiLaValidacionFalla()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync(TipoSala.Imax);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,sala.Id,new DateTime(2024,5,6,20,0,0),Formato.TresD),
                    CancellationToken.None));

            Assert.Empty(await _db.Context.Funciones.ToListAsync());
        }

        [Fact]
        public async Task GuardaLaFuncionEnElRepositorio()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var id = await CrearHandler().Handle(
                CrearComando(pelicula.Id,sala.Id,new DateTime(2024,5,6,20,0,0)),
                CancellationToken.None);

            var obtener = await new FuncionRepository(_db.Context)
                .ObtenerPorIdAsync(id,CancellationToken.None);

            Assert.NotNull(obtener);
            Assert.Equal(id,obtener!.Id);
        }
    }
}
