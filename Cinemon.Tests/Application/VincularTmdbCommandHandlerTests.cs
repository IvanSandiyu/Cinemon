using Cinemon.Application.DTOs.Tmdb;
using Cinemon.Application.Interfaces;
using Cinemon.Application.Peliculas.Commands.VincularTmdb;
using Cinemon.Domain.Entidades.Peliculas;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    public class VincularTmdbCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private sealed class TmdbServiceFake(
            TmdbMovieDto resultado) : ITmdbService
        {
            public Task<TmdbMovieDto?> ObtenerPeliculaAsync(int tmdbId, CancellationToken cancellationToken) =>
                Task.FromResult<TmdbMovieDto?>(resultado);

            public Task<IReadOnlyCollection<TmdbMovieDto>> BuscarPeliculasAsync(string query, CancellationToken cancellationToken) =>
                Task.FromResult<IReadOnlyCollection<TmdbMovieDto>>([]);
        }

        private static TmdbMovieDto CrearResultado(
            int tmdbId = 123,
            IReadOnlyCollection<string>? posters = null) =>
            new(
                tmdbId,
                "Pelicula TMDB",
                "Sinopsis.",
                "/poster.jpg",
                "/backdrop.jpg",
                new DateTime(2024, 5, 6),
                118,
                "ATP",
                "trailer_key",
                [],
                posters ?? ["/p1.jpg", "/p2.jpg", "/p1.jpg"]);

        private VincularTmdbCommandHandler CrearHandler(TmdbMovieDto resultado) =>
            new(
                new PeliculaRepository(_db.Context),
                new TmdbServiceFake(resultado),
                new GeneroRepository(_db.Context));

        private async Task<Pelicula> CrearPelicula()
        {
            var pelicula = await _db.CrearPeliculaAsync();

            _db.Context.Entry(pelicula).State = EntityState.Detached;

            return pelicula;
        }

        [Fact]
        public async Task GuardaTodosLosPostersQueDevuelveTmdbSinDuplicados()
        {
            var pelicula = await CrearPelicula();

            await CrearHandler(CrearResultado()).Handle(
                new VincularTmdbCommand(pelicula.Id, 123),
                CancellationToken.None);

            var posters = await _db.Context.Set<PeliculaPoster>()
                .AsNoTracking()
                .Where(x => x.PeliculaId == pelicula.Id)
                .Select(x => x.Ruta)
                .OrderBy(x => x)
                .ToListAsync();

            Assert.Equal(["/p1.jpg", "/p2.jpg"], posters);
        }

        [Fact]
        public async Task GuardaElTmdbIdYElPosterPrincipal()
        {
            var pelicula = await CrearPelicula();

            await CrearHandler(CrearResultado()).Handle(
                new VincularTmdbCommand(pelicula.Id, 123),
                CancellationToken.None);

            var guardada = await _db.Context.Peliculas
                .AsNoTracking()
                .SingleAsync(x => x.Id == pelicula.Id);

            Assert.Equal(123, guardada.TmdbId);
            Assert.Equal("/poster.jpg", guardada.TmdbPosterPath);
        }

        [Fact]
        public async Task ReemplazaLosPostersAlVolverAVincular()
        {
            var pelicula = await CrearPelicula();

            var handler = CrearHandler(CrearResultado(posters: ["/p1.jpg", "/p2.jpg"]));

            await handler.Handle(
                new VincularTmdbCommand(pelicula.Id, 123),
                CancellationToken.None);

            _db.Context.ChangeTracker.Clear();

            var handlerDos = CrearHandler(CrearResultado(posters: ["/p3.jpg"]));

            await handlerDos.Handle(
                new VincularTmdbCommand(pelicula.Id, 123),
                CancellationToken.None);

            var rutas = await _db.Context.Set<PeliculaPoster>()
                .AsNoTracking()
                .Where(x => x.PeliculaId == pelicula.Id)
                .Select(x => x.Ruta)
                .ToListAsync();

            Assert.Equal(["/p3.jpg"], rutas);
        }

        [Fact]
        public async Task SiTmdbNoTraePostersLaColeccionQuedaVacia()
        {
            var pelicula = await CrearPelicula();

            await CrearHandler(CrearResultado(posters: ["/p1.jpg"])).Handle(
                new VincularTmdbCommand(pelicula.Id, 123),
                CancellationToken.None);

            _db.Context.ChangeTracker.Clear();

            await CrearHandler(CrearResultado(posters: [])).Handle(
                new VincularTmdbCommand(pelicula.Id, 123),
                CancellationToken.None);

            var cantidad = await _db.Context.Set<PeliculaPoster>()
                .AsNoTracking()
                .CountAsync(x => x.PeliculaId == pelicula.Id);

            Assert.Equal(0, cantidad);
        }
    }
}