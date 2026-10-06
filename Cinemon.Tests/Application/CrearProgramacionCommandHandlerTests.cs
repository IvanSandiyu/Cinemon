using Cinemon.Application.Funciones.Commands.CrearProgramacion;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
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
    public class CrearProgramacionCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db = new();

        void IDisposable.Dispose() => _db.Dispose();

        private CrearProgramacionCommandHandler CrearHandler() =>
            new(
                new PeliculaRepository(_db.Context),
                new SalaRepository(_db.Context),
                new FuncionRepository(_db.Context));

        /// <summary>
        /// Empieza el lunes siguiente al anchor, para que todas las fechas
        /// generadas queden en el futuro.
        /// </summary>
        private static DateTime LunesFuturo()
        {
            var hoy = DateTime.UtcNow.Date;

            var dias = (int)DayOfWeek.Monday - (int)hoy.DayOfWeek;

            if (dias <= 0)
                dias += 7;

            return hoy.AddDays(dias);
        }

        private static CrearProgramacionCommand CrearComando(
            int peliculaId,
            int salaId,
            DateTime fechaInicio,
            IReadOnlyCollection<DayOfWeek> diasSemana,
            int semanas,
            Formato formato = Formato.DosD) =>
            new(
                peliculaId,
                salaId,
                fechaInicio,
                diasSemana,
                semanas,
                IdiomaFuncion.Espanol,
                formato,
                10000m);

        [Fact]
        public async Task CreaUnaFuncionPorCadaDiaYSemanaSeleccionados()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var resultado = await CrearHandler().Handle(
                CrearComando(
                    pelicula.Id,
                    sala.Id,
                    LunesFuturo(),
                    [DayOfWeek.Monday,DayOfWeek.Wednesday,DayOfWeek.Friday],
                    2),
                CancellationToken.None);

            Assert.Equal(6,resultado.Creadas.Count);
            Assert.Empty(resultado.Omitidas);
            Assert.Equal(6,await _db.Context.Funciones.CountAsync());
        }

        [Fact]
        public async Task LasFuncionesQuedanOrdenadasPorFecha()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var resultado = await CrearHandler().Handle(
                CrearComando(
                    pelicula.Id,
                    sala.Id,
                    LunesFuturo(),
                    [DayOfWeek.Friday,DayOfWeek.Monday,DayOfWeek.Wednesday],
                    2),
                CancellationToken.None);

            var fechas = resultado.Creadas.Select(x => x.FechaHoraInicio).ToArray();

            Assert.Equal(fechas.OrderBy(x => x),fechas);
        }

        [Fact]
        public async Task RespetaLaHoraDeLaFechaDeInicio()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var inicio = LunesFuturo().AddHours(21).AddMinutes(30);

            var resultado = await CrearHandler().Handle(
                CrearComando(pelicula.Id,sala.Id,inicio,[DayOfWeek.Monday],1),
                CancellationToken.None);

            var unica = Assert.Single(resultado.Creadas);

            Assert.Equal(21,unica.FechaHoraInicio.Hour);
            Assert.Equal(30,unica.FechaHoraInicio.Minute);
        }

        [Fact]
        public async Task OmiteLasFechasQueYaSuperponenConUnaFuncionExistente()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var lunes = LunesFuturo();

            // Misma hora que la programación, así se superpone con la de esa semana.
            await _db.CrearFuncionAsync(pelicula.Id,sala.Id,lunes);

            var resultado = await CrearHandler().Handle(
                CrearComando(pelicula.Id,sala.Id,lunes,[DayOfWeek.Monday],2),
                CancellationToken.None);

            Assert.Single(resultado.Creadas);
            Assert.Single(resultado.Omitidas);
            Assert.Equal(2,resultado.Creadas.Count + resultado.Omitidas.Count);

            // La original + la única que sí se pudo crear.
            Assert.Equal(2,await _db.Context.Funciones.CountAsync());
        }

        [Fact]
        public async Task OmiteLasFechasQueYaPasaron()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var hoy = DateTime.UtcNow.Date;

            // El lunes de la semana pasada ya pasó.
            var resultado = await CrearHandler().Handle(
                CrearComando(
                    pelicula.Id,
                    sala.Id,
                    hoy.AddDays(-((int)DayOfWeek.Monday - (int)hoy.DayOfWeek + 7) % 7 - 7),
                    [DayOfWeek.Monday],
                    1),
                CancellationToken.None);

            Assert.Empty(resultado.Creadas);
            Assert.Single(resultado.Omitidas);
            Assert.Empty(await _db.Context.Funciones.ToListAsync());
        }

        [Fact]
        public async Task NoRepiteElMismoDiaSiSePasaVariasVeces()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var resultado = await CrearHandler().Handle(
                CrearComando(
                    pelicula.Id,
                    sala.Id,
                    LunesFuturo(),
                    [DayOfWeek.Monday,DayOfWeek.Monday],
                    1),
                CancellationToken.None);

            Assert.Single(resultado.Creadas);
        }

        [Fact]
        public async Task FallaSiLaPeliculaNoExiste()
        {
            var sala = await _db.CrearSalaAsync();

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler().Handle(
                    CrearComando(9999,sala.Id,LunesFuturo(),[DayOfWeek.Monday],1),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiLaPeliculaEstaInactiva()
        {
            var pelicula = await _db.CrearPeliculaAsync(activa: false);
            var sala = await _db.CrearSalaAsync();

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,sala.Id,LunesFuturo(),[DayOfWeek.Monday],1),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiLaSalaNoExiste()
        {
            var pelicula = await _db.CrearPeliculaAsync();

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,9999,LunesFuturo(),[DayOfWeek.Monday],1),
                    CancellationToken.None));
        }

        [Fact]
        public async Task FallaSiLaSalaEsImaxYLaFuncionEsTresD()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync(TipoSala.Imax);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,sala.Id,LunesFuturo(),[DayOfWeek.Monday],1,Formato.TresD),
                    CancellationToken.None));

            Assert.Empty(await _db.Context.Funciones.ToListAsync());
        }

        [Fact]
        public async Task FallaSiNoSeSeleccionoNingunDia()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler().Handle(
                    CrearComando(pelicula.Id,sala.Id,LunesFuturo(),[],1),
                    CancellationToken.None));
        }
    }
}
