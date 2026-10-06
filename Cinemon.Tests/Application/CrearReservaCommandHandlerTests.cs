using Cinemon.Application.Abstractions;
using Cinemon.Application.Interfaces;
using Cinemon.Application.Reservas.Commands.CrearReserva;
using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Fakes;
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
    public class CrearReservaCommandHandlerTests : IDisposable
    {
        private readonly TestDbContext _db;
        private readonly UsuarioRepository _usuarios;
        private readonly FuncionRepository _funciones;
        private readonly ButacaRepository _butacas;
        private readonly ReservaRepository _reservas;
        private readonly PromocionRepository _promociones;

        // 2024-05-06 fue un lunes.
        private static readonly DateTime Lunes = new(2024,5,6,20,0,0);

        public CrearReservaCommandHandlerTests()
        {
            _db = new TestDbContext();

            _usuarios = new UsuarioRepository(_db.Context);
            _funciones = new FuncionRepository(_db.Context);
            _butacas = new ButacaRepository(_db.Context);
            _reservas = new ReservaRepository(_db.Context);
            _promociones = new PromocionRepository(_db.Context);
        }

        public void Dispose() => _db.Dispose();

        private CrearReservaCommandHandler CrearHandler(int userId) =>
            new(_usuarios,_funciones,_butacas,_reservas,_promociones,new FakeCurrentUserService(userId));

        // --------------------------------------------------------------
        // Happy path
        // --------------------------------------------------------------

        [Fact]
        public async Task CreaLaReservaYCobraElSubtotalCompletoSinPromociones()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes,precio: 10000m);

            var butacas = new List<Butaca>
            {
                await _db.CrearButacaAsync(sala.Id,"A",1),
                await _db.CrearButacaAsync(sala.Id,"A",2)
            };

            var ids = butacas.Select(x => x.Id).ToList();

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,ids),CancellationToken.None);

            Assert.True(reservaId > 0);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            Assert.Equal(20000m,reserva.Total);
            Assert.Equal(EstadoReserva.Confirmada,reserva.EstadoReserva);
            Assert.Equal(usuario.Id,reserva.UsuarioId);

            var butacasReservadas = await _db.Context.ReservasButacas
                .AsNoTracking()
                .Where(x => x.ReservaId == reservaId)
                .ToListAsync();

            Assert.Equal(2,butacasReservadas.Count);
        }

        [Fact]
        public async Task CobraElPrecioDeLaFuncionYTipoSalaYSala()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync(TipoSala.Imax);
            var funcion = await _db.CrearFuncionAsync(
                pelicula.Id,sala.Id,Lunes,precio: 12000m,formato: Formato.CuatroD);

            var butaca = await _db.CrearButacaAsync(sala.Id);

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,[butaca.Id]),CancellationToken.None);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            Assert.Equal(12000m,reserva.Total);
        }

        // --------------------------------------------------------------
        // Promociones
        // --------------------------------------------------------------

        [Theory]
        [InlineData(0)] // lunes 2024-05-06
        [InlineData(1)] // martes
        [InlineData(2)] // miércoles
        public async Task AplicaEl2x1EnLunesMartesYMiercoles(int offset)
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var funcion = await _db.CrearFuncionAsync(
                pelicula.Id,sala.Id,Lunes.AddDays(offset),precio: 10000m);

            await _db.CrearPromocionNxMAsync(cantidadPagadas: 2,cantidadGratis: 1);

            var butacas = new List<int>
            {
                (await _db.CrearButacaAsync(sala.Id,"A",1)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",2)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",3)).Id
            };

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,butacas),CancellationToken.None);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            Assert.Equal(20000m,reserva.Total);
        }

        [Fact]
        public async Task NoAplicaEl2x1EnJueves()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var jueves = new DateTime(2024,5,9,20,0,0);

            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,jueves,precio: 10000m);

            await _db.CrearPromocionNxMAsync();

            var butacas = new List<int>
            {
                (await _db.CrearButacaAsync(sala.Id,"A",1)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",2)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",3)).Id
            };

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,butacas),CancellationToken.None);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            Assert.Equal(30000m,reserva.Total);
        }

        [Fact]
        public async Task El2x1NoDescuentaConDosEntradas()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes,precio: 10000m);

            await _db.CrearPromocionNxMAsync();

            var butacas = new List<int>
            {
                (await _db.CrearButacaAsync(sala.Id,"A",1)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",2)).Id
            };

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,butacas),CancellationToken.None);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            Assert.Equal(20000m,reserva.Total);
        }

        [Fact]
        public async Task AplicaPromocionDePorcentaje()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes,precio: 10000m);

            await _db.CrearPromocionPorcentajeAsync(porcentaje: 25m);

            var butacas = new List<int>
            {
                (await _db.CrearButacaAsync(sala.Id,"A",1)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",2)).Id
            };

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,butacas),CancellationToken.None);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            Assert.Equal(15000m,reserva.Total);
        }

        [Fact]
        public async Task CobraLaPromocionMasBeneficiosa()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes,precio: 10000m);

            await _db.CrearPromocionNxMAsync(nombre: "2x1",cantidadPagadas: 2,cantidadGratis: 1);
            await _db.CrearPromocionPorcentajeAsync(nombre: "10% off",porcentaje: 10m);

            var butacas = new List<int>
            {
                (await _db.CrearButacaAsync(sala.Id,"A",1)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",2)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",3)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",4)).Id
            };

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,butacas),CancellationToken.None);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            // 2x1 -> pagan 3 = 30000; 10% -> 36000. Gana el 2x1.
            Assert.Equal(30000m,reserva.Total);
        }

        [Fact]
        public async Task NoAplicaPromocionesInactivas()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes,precio: 10000m);

            await _db.CrearPromocionNxMAsync(activa: false);

            var butacas = new List<int>
            {
                (await _db.CrearButacaAsync(sala.Id,"A",1)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",2)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",3)).Id
            };

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,butacas),CancellationToken.None);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            Assert.Equal(30000m,reserva.Total);
        }

        [Fact]
        public async Task NoAplicaPromocionesFueraDeVigencia()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes,precio: 10000m);

            await _db.CrearPromocionNxMAsync(
                desde: new DateTime(2020,1,1),
                hasta: new DateTime(2020,12,31));

            var butacas = new List<int>
            {
                (await _db.CrearButacaAsync(sala.Id,"A",1)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",2)).Id,
                (await _db.CrearButacaAsync(sala.Id,"A",3)).Id
            };

            var reservaId = await CrearHandler(usuario.Id)
                .Handle(new CrearReservaCommand(funcion.Id,butacas),CancellationToken.None);

            var reserva = await _db.Context.Reservas
                .AsNoTracking()
                .SingleAsync(x => x.Id == reservaId);

            Assert.Equal(30000m,reserva.Total);
        }

        // --------------------------------------------------------------
        // Validaciones
        // --------------------------------------------------------------

        [Fact]
        public async Task Falla_SiElUsuarioNoExiste()
        {
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes);
            var butaca = await _db.CrearButacaAsync(sala.Id);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler(9999).Handle(
                    new CrearReservaCommand(funcion.Id,[butaca.Id]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task Falla_SiElUsuarioEstaInactivo()
        {
            var usuario = await _db.CrearUsuarioAsync(activo: false);
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes);
            var butaca = await _db.CrearButacaAsync(sala.Id);

            var excepcion = await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearReservaCommand(funcion.Id,[butaca.Id]),
                    CancellationToken.None));

            Assert.Contains("activo",excepcion.Message);
        }

        [Fact]
        public async Task Falla_SiLaFuncionNoExiste()
        {
            var usuario = await _db.CrearUsuarioAsync();

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearReservaCommand(9999,[1]),
                    CancellationToken.None));
        }

        [Theory]
        [InlineData(EstadoFuncion.Cancelada)]
        [InlineData(EstadoFuncion.Finalizada)]
        public async Task Falla_SiLaFuncionNoEstaProgramada(EstadoFuncion estado)
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();

            var funcion = await _db.CrearFuncionAsync(
                pelicula.Id,sala.Id,Lunes,estado: estado);

            var butaca = await _db.CrearButacaAsync(sala.Id);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearReservaCommand(funcion.Id,[butaca.Id]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task Falla_SiAlgunaButacaNoExiste()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes);

            var butaca = await _db.CrearButacaAsync(sala.Id);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearReservaCommand(funcion.Id,[butaca.Id,9999]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task Falla_SiAlgunaButacaNoPerteneceAlaSala()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var otraSala = await _db.CrearSalaAsync();

            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes);

            var butacaSala = await _db.CrearButacaAsync(sala.Id,"A",1);
            var butacaOtraSala = await _db.CrearButacaAsync(otraSala.Id,"A",1);

            await Assert.ThrowsAsync<BusinessRuleException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearReservaCommand(funcion.Id,[butacaSala.Id,butacaOtraSala.Id]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task Falla_SiLaButacaYaEstaReservada()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes);

            var butaca = await _db.CrearButacaAsync(sala.Id);

            await CrearHandler(usuario.Id).Handle(
                new CrearReservaCommand(funcion.Id,[butaca.Id]),
                CancellationToken.None);

            await Assert.ThrowsAsync<ConflictException>(() =>
                CrearHandler(usuario.Id).Handle(
                    new CrearReservaCommand(funcion.Id,[butaca.Id]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task Falla_SiUnaButacaEstaReservadaPorOtroUsuario()
        {
            var usuario1 = await _db.CrearUsuarioAsync();
            var usuario2 = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes);

            var butaca1 = await _db.CrearButacaAsync(sala.Id,"A",1);
            var butaca2 = await _db.CrearButacaAsync(sala.Id,"A",2);

            await CrearHandler(usuario1.Id).Handle(
                new CrearReservaCommand(funcion.Id,[butaca1.Id]),
                CancellationToken.None);

            await Assert.ThrowsAsync<ConflictException>(() =>
                CrearHandler(usuario2.Id).Handle(
                    new CrearReservaCommand(funcion.Id,[butaca2.Id,butaca1.Id]),
                    CancellationToken.None));
        }

        [Fact]
        public async Task Cancelar_LiberaLasButacas()
        {
            var usuario = await _db.CrearUsuarioAsync();
            var pelicula = await _db.CrearPeliculaAsync();
            var sala = await _db.CrearSalaAsync();
            var funcion = await _db.CrearFuncionAsync(pelicula.Id,sala.Id,Lunes);

            var butaca = await _db.CrearButacaAsync(sala.Id);

            var reservaId = await CrearHandler(usuario.Id).Handle(
                new CrearReservaCommand(funcion.Id,[butaca.Id]),
                CancellationToken.None);

            Assert.True(await _reservas.CancelarAsync(reservaId,CancellationToken.None));

            Assert.True(await _reservas.ButacasDisponiblesAsync(
                funcion.Id,[butaca.Id],CancellationToken.None));

            // La butaca liberada se puede volver a reservar.
            var nuevaReservaId = await CrearHandler(usuario.Id).Handle(
                new CrearReservaCommand(funcion.Id,[butaca.Id]),
                CancellationToken.None);

            Assert.True(nuevaReservaId > 0);
        }
    }
}
