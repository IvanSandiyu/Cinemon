using Cinemon.Application.Reservas.Commands.CrearReserva;
using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using Cinemon.Infrastructure;
using Cinemon.Infrastructure.Repositories;
using Cinemon.Tests.Fakes;
using Cinemon.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Cinemon.Tests.Application
{
    /// <summary>
    /// El peor caso del sistema: dos usuarios reservan la misma butaca
    /// al mismo tiempo. Hay que garantizar que gana exactamente uno.
    /// </summary>
    public class ReservaConcurrencyTests
    {
        private static readonly DateTime Lunes = new(2024,5,6,20,0,0);

        private sealed class Escenario : IDisposable
        {
            private readonly SqliteTestDatabase _db = new();

            public int FuncionId { get; private set; }

            public int SalaId { get; private set; }

            public CinemonDbContext CrearContexto() => _db.CreateContext();
            public async Task PrepararAsync()
            {
                await using var contexto = CrearContexto();

                var pelicula = new Cinemon.Domain.Entidades.Peliculas.Pelicula(
                    "Pelicula",
                    "Sinopsis",
                    120,
                    DateTime.UtcNow.AddMonths(-1),
                    ClasificacionEdad.ATP,
                    "poster",
                    "trailer");

                pelicula.Activar();

                contexto.Peliculas.Add(pelicula);

                await contexto.SaveChangesAsync();

                var sala = new Cinemon.Domain.Entidades.Salas.Sala(1,TipoSala.Estandar);

                contexto.Salas.Add(sala);

                await contexto.SaveChangesAsync();

                SalaId = sala.Id;

                var funcion = new Cinemon.Domain.Entidades.Funcion.Funcion(
                    pelicula.Id,sala.Id,Lunes,IdiomaFuncion.Espanol,Formato.DosD,10000m);

                contexto.Funciones.Add(funcion);

                await contexto.SaveChangesAsync();

                FuncionId = funcion.Id;
            }

            public async Task CrearUsuariosAsync(params int[] ids)
            {
                await using var contexto = CrearContexto();

                foreach (var id in ids)
                {
                    contexto.Usuarios.Add(
                        new Cinemon.Domain.Entidades.Usuarios.Usuario(
                            $"Usuario {id}",
                            $"u{id}@test.com",
                            Rol.Cliente,
                            "hash"));
                }

                await contexto.SaveChangesAsync();
            }

            public void Dispose() => _db.Dispose();
        }

        private static CrearReservaCommandHandler CrearHandler(CinemonDbContext contexto,int userId)
        {
            return new CrearReservaCommandHandler(
                new UsuarioRepository(contexto),
                new FuncionRepository(contexto),
                new ButacaRepository(contexto),
                new ReservaRepository(contexto),
                new PromocionRepository(contexto),
                new FakeCurrentUserService(userId));
        }

        /// <summary>
        /// Intenta reservar y devuelve true si gana, false si recibe
        /// el conflicto de butaca ya reservada.
        ///
        /// Todos los participants esperan la misma compuerta para salir
        /// juntos. La compuerta es asíncrica a propósito: con un
        /// <see cref="Barrier"/> (que bloquea) el test se deadlockea,
        /// porque xUnit serializa las continuaciones de cada test.
        /// </summary>
        private static Task<bool> IntentarReservar(
            Escenario escenario,
            int userId,
            Task compuerta,
            IReadOnlyCollection<int> butacasIds)
        {
            return Task.Run(async () =>
            {
                await compuerta.ConfigureAwait(false);

                await using var contexto = escenario.CrearContexto();

                var handler = CrearHandler(contexto,userId);

                try {
                    await handler.Handle(
                        new CrearReservaCommand(escenario.FuncionId,butacasIds),
                        CancellationToken.None);

                    return true;
                } catch (ConflictException) {
                    return false;
                }
            });
        }

        /// <summary>
        /// Arranca las reservas en paralelo y devuelve cuántos ganaron.
        /// </summary>
        private static async Task<bool[]> CorrerEnParalelo(
            Escenario escenario,
            int cantidadUsuarios,
            IReadOnlyCollection<int> butacasIds)
        {
            var compuerta = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

            var tareas = Enumerable
                .Range(1,cantidadUsuarios)
                .Select(userId => IntentarReservar(escenario,userId,compuerta.Task,butacasIds))
                .ToArray();

            compuerta.SetResult();

            return await Task.WhenAll(tareas);
        }

        [Fact]
        public async Task TresUsuariosEnParalelo_SoloUnoLograReservar()
        {
            using var escenario = new Escenario();

            await escenario.PrepararAsync();

            await escenario.CrearUsuariosAsync(1,2,3);

            var butacaId = await CrearButacaAsync(escenario,"A",1);

            var resultados = await CorrerEnParalelo(escenario,3,[butacaId]);

            Assert.Equal(1,resultados.Count(x => x));
            Assert.Equal(2,resultados.Count(x => !x));

            await using var verificacion = escenario.CrearContexto();

            var butacasReservadas = await verificacion.ReservasButacas
                .AsNoTracking()
                .Where(x => x.FuncionId == escenario.FuncionId)
                .ToListAsync();

            Assert.Single(butacasReservadas);
        }

        [Fact]
        public async Task DosUsuariosEnParalelo_SoloUnoLograReservar()
        {
            using var escenario = new Escenario();

            await escenario.PrepararAsync();

            await escenario.CrearUsuariosAsync(1,2);

            var butacaId = await CrearButacaAsync(escenario,"A",1);

            var resultados = await CorrerEnParalelo(escenario,2,[butacaId]);

            Assert.Equal(1,resultados.Count(x => x));
            Assert.Equal(1,resultados.Count(x => !x));

            await using var verificacion = escenario.CrearContexto();

            Assert.Equal(1,await verificacion.ReservasButacas
                .AsNoTracking()
                .CountAsync(x => x.FuncionId == escenario.FuncionId));
        }

        [Fact]
        public async Task DosUsuariosEnParalelo_SobreButacasCompartidas_GanaSoloLaReservaCompleta()
        {
            using var escenario = new Escenario();

            await escenario.PrepararAsync();

            await escenario.CrearUsuariosAsync(1,2);

            var butacaA = await CrearButacaAsync(escenario,"A",1);
            var butacaB = await CrearButacaAsync(escenario,"A",2);

            var butacas = new List<int>
            {
                butacaA,
                butacaB
            };

            var resultados = await CorrerEnParalelo(escenario,2,butacas);

            Assert.Equal(1,resultados.Count(x => x));

            await using var verificacion = escenario.CrearContexto();

            var reservadas = await verificacion.ReservasButacas
                .AsNoTracking()
                .Where(x => x.FuncionId == escenario.FuncionId)
                .ToListAsync();

            // La transacción del perdedor no deja butacas sueltas.
            Assert.Equal(2,reservadas.Count);
            Assert.Equal(2,reservadas.Select(x => x.ButacaId).Distinct().Count());
        }

        [Fact]
        public async Task ElIndiceUnicoBloqueaLaDobleReservaAunSinLaValidacionPrevia()
        {
            using var escenario = new Escenario();

            await escenario.PrepararAsync();

            await escenario.CrearUsuariosAsync(1);

            var butacaId = await CrearButacaAsync(escenario,"A",1);

            await using var contexto = escenario.CrearContexto();

            var reserva = new Cinemon.Domain.Entidades.Reservas.Reserva(
                1,1,escenario.FuncionId,10000m);

            contexto.Reservas.Add(reserva);

            await contexto.SaveChangesAsync();

            contexto.ReservasButacas.Add(
                new ReservaButaca(reserva.Id,escenario.FuncionId,butacaId));

            await contexto.SaveChangesAsync();

            contexto.ReservasButacas.Add(
                new ReservaButaca(reserva.Id,escenario.FuncionId,butacaId));

            await Assert.ThrowsAsync<DbUpdateException>(() => contexto.SaveChangesAsync());
        }

        private static async Task<int> CrearButacaAsync(Escenario escenario,string fila,int numero)
        {
            await using var contexto = escenario.CrearContexto();

            var butaca = new Butaca(escenario.SalaId,fila,numero);

            contexto.Butacas.Add(butaca);

            await contexto.SaveChangesAsync();

            return butaca.Id;
        }
    }
}
