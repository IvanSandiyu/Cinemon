using Cinemon.Application.Common;
using Cinemon.Application.Peliculas.Queries.ObtenerPeliculas;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Infrastructure;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    public class ObtenerPeliculasPaginadasQueryHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private ObtenerPeliculasPaginadasQueryHandler CrearHandler() =>
            new(
                new PeliculaRepository(_db.Context),
                TimeProvider.System);

        private Task<PagedResult<PeliculaDto>> Obtener(
            int pagina = 1,
            int tamanoPagina = 9,
            FiltroPeliculasPublicas filtro = FiltroPeliculasPublicas.EnCartelera,
            string? busqueda = null,
            IReadOnlyCollection<int>? ids = null) =>
            CrearHandler().Handle(
                new ObtenerPeliculasPaginadasQuery(
                    pagina,
                    tamanoPagina,
                    filtro,
                    busqueda,
                    ids),
                CancellationToken.None);

        [Fact]
        public async Task PaginaLaCarteleraYDevuelveElTotal()
        {
            for (var i = 0; i < 10; i++)
                await _db.CrearPeliculaAsync(activa: true);

            var primera = await Obtener(pagina: 1, tamanoPagina: 9);
            var segunda = await Obtener(pagina: 2, tamanoPagina: 9);

            Assert.Equal(10, primera.Total);
            Assert.Equal(2, primera.TotalPaginas);
            Assert.Equal(9, primera.Items.Count);
            Assert.Single(segunda.Items);
        }

        [Fact]
        public async Task EnCarteleraSoloDevuelvePeliculasActivas()
        {
            await _db.CrearPeliculaAsync(activa: true);
            await _db.CrearPeliculaAsync(activa: false);

            var resultado = await Obtener(
                filtro: FiltroPeliculasPublicas.EnCartelera);

            Assert.Single(resultado.Items);
            Assert.True(resultado.Items.Single().Activa);
        }

        [Fact]
        public async Task ProximamenteFiltraPorFechaFuturaYOrdenaPorEstreno()
        {
            var hoy = TimeProvider.System.Ahora().Date;

            await _db.CrearPeliculaAsync(
                titulo: "Pasada",
                fechaEstreno: hoy.AddDays(-1));

            var segunda = await _db.CrearPeliculaAsync(
                titulo: "Segunda",
                fechaEstreno: hoy.AddDays(10));

            var primera = await _db.CrearPeliculaAsync(
                titulo: "Primera",
                fechaEstreno: hoy.AddDays(5));

            var resultado = await Obtener(
                filtro: FiltroPeliculasPublicas.Proximamente);

            Assert.Equal(2, resultado.Total);
            Assert.Equal(
                new[] { primera.Id, segunda.Id },
                resultado.Items.Select(p => p.Id));
        }

        [Fact]
        public async Task BusquedaFiltraPorTituloSoloActivasOProximas()
        {
            var hoy = TimeProvider.System.Ahora().Date;

            await _db.CrearPeliculaAsync(
                titulo: "Matrix activa",
                activa: true);

            await _db.CrearPeliculaAsync(
                titulo: "Matrix futura",
                activa: false,
                fechaEstreno: hoy.AddDays(5));

            await _db.CrearPeliculaAsync(
                titulo: "Matrix inactiva",
                activa: false,
                fechaEstreno: hoy.AddDays(-5));

            await _db.CrearPeliculaAsync(
                titulo: "Otra pelicula",
                activa: true);

            var resultado = await Obtener(
                filtro: FiltroPeliculasPublicas.Busqueda,
                busqueda: "Matrix");

            Assert.Equal(2, resultado.Total);
            Assert.All(
                resultado.Items,
                p => Assert.Contains("Matrix", p.Titulo));
        }

        [Fact]
        public async Task MisPeliculasFiltraPorIdsYPagina()
        {
            var primera = await _db.CrearPeliculaAsync(activa: true);
            var segunda = await _db.CrearPeliculaAsync(activa: true);

            await _db.CrearPeliculaAsync(activa: true);

            var primeraPagina = await Obtener(
                filtro: FiltroPeliculasPublicas.MisPeliculas,
                ids: [segunda.Id, primera.Id],
                pagina: 1,
                tamanoPagina: 1);

            var segundaPagina = await Obtener(
                filtro: FiltroPeliculasPublicas.MisPeliculas,
                ids: [segunda.Id, primera.Id],
                pagina: 2,
                tamanoPagina: 1);

            Assert.Equal(2, primeraPagina.Total);
            Assert.Equal(2, primeraPagina.TotalPaginas);
            Assert.Equal(primera.Id, primeraPagina.Items.Single().Id);
            Assert.Equal(segunda.Id, segundaPagina.Items.Single().Id);
        }

        [Fact]
        public async Task AjustaPaginaYTamanoFueraDeRango()
        {
            for (var i = 0; i < 3; i++)
                await _db.CrearPeliculaAsync(activa: true);

            var resultado = await Obtener(pagina: 0, tamanoPagina: 0);

            Assert.Equal(1, resultado.Pagina);
            Assert.Equal(1, resultado.TamanoPagina);
            Assert.Single(resultado.Items);
        }
    }
}
