using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Entidades.Generos;
using Cinemon.Domain.Entidades.Peliculas;
using Cinemon.Domain.Entidades.Precios;
using Cinemon.Domain.Entidades.Promociones;
using Cinemon.Domain.Entidades.Salas;
using Cinemon.Domain.Entidades.Usuarios;
using Cinemon.Domain.Enums;
using Cinemon.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinemon.Tests.Infrastructure
{
    /// <summary>
    /// Base de datos SQLite en memoria para los tests.
    /// Se usa SQLite (y no el provider InMemory) a propósito:
    /// el provider InMemory no respeta índices únicos ni transacciones,
    /// y justamente necesitamos esas garantías para probar la
    /// concurrencia de reservas.
    /// </summary>
    public sealed class TestDbContext : IDisposable
    {
        private readonly SqliteConnection _connection;

        public TestDbContext()
        {
            _connection = new SqliteConnection("DataSource=:memory:");

            _connection.Open();

            Context = new CinemonDbContext(
                new DbContextOptionsBuilder<CinemonDbContext>()
                    .UseSqlite(_connection)
                    .Options);

            Context.Database.EnsureCreated();
        }

        public CinemonDbContext Context { get; }

        public void Dispose() => _connection.Dispose();

        // ------------------------------------------------------------------
        // Helpers de seed
        // ------------------------------------------------------------------

        public async Task<Sala> CrearSalaAsync(TipoSala tipoSala = TipoSala.Estandar)
        {
            var sala = new Sala(
                Context.Salas.Count() + 1,
                tipoSala);

            Context.Salas.Add(sala);

            await Context.SaveChangesAsync();

            return sala;
        }

        public async Task<Butaca> CrearButacaAsync(int salaId,string fila = "A",int numero = 1)
        {
            var butaca = new Butaca(salaId,fila,numero);

            Context.Butacas.Add(butaca);

            await Context.SaveChangesAsync();

            return butaca;
        }

        public async Task<Pelicula> CrearPeliculaAsync(
            bool activa = true,
            int duracion = 120)
        {
            var pelicula = new Pelicula(
                $"Pelicula {Guid.NewGuid():N}"[..12],
                "Sinopsis de prueba.",
                duracion,
                DateTime.UtcNow.AddMonths(-1),
                ClasificacionEdad.ATP,
                "https://image.tmdb.org/poster.jpg",
                "https://youtube.com/watch?v=test");

            if (activa)
                pelicula.Activar();

            Context.Peliculas.Add(pelicula);

            await Context.SaveChangesAsync();

            return pelicula;
        }

        public async Task<Usuario> CrearUsuarioAsync(Rol rol = Rol.Cliente,bool activo = true)
        {
            var usuario = new Usuario(
                $"Usuario {Guid.NewGuid():N}"[..14],
                $"{Guid.NewGuid():N}@test.com",
                rol,
                "hash-de-prueba");

            if (!activo)
                usuario.Desactivar();

            Context.Usuarios.Add(usuario);

            await Context.SaveChangesAsync();

            return usuario;
        }

        public async Task<Funcion> CrearFuncionAsync(
            int peliculaId,
            int salaId,
            DateTime fechaHoraInicio,
            decimal precio = 10000m,
            Formato formato = Formato.DosD,
            IdiomaFuncion idioma = IdiomaFuncion.Espanol,
            EstadoFuncion estado = EstadoFuncion.Programada)
        {
            var funcion = new Funcion(
                peliculaId,
                salaId,
                fechaHoraInicio,
                idioma,
                formato,
                precio);

            switch (estado)
            {
                case EstadoFuncion.Cancelada:
                    funcion.Cancelar();
                    break;

                case EstadoFuncion.Finalizada:
                    funcion.Finalizar();
                    break;
            }

            Context.Funciones.Add(funcion);

            await Context.SaveChangesAsync();

            return funcion;
        }

        public async Task<Promocion> CrearPromocionNxMAsync(
            string nombre = "2x1",
            int cantidadPagadas = 2,
            int cantidadGratis = 1,
            DateTime? desde = null,
            DateTime? hasta = null,
            bool activa = true)
        {
            var promocion = new Promocion(
                nombre,
                "Promoción de prueba NxM.",
                TipoPromocion.NxM,
                cantidadPagadas,
                cantidadGratis,
                null,
                desde ?? new DateTime(2020,1,1),
                hasta ?? new DateTime(2099,12,31),
                [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday]);

            if (!activa)
                promocion.Desactivar();

            Context.Promociones.Add(promocion);

            await Context.SaveChangesAsync();

            return promocion;
        }

        public async Task<Promocion> CrearPromocionPorcentajeAsync(
            string nombre = "20% off",
            decimal porcentaje = 20m,
            DateTime? desde = null,
            DateTime? hasta = null,
            bool activa = true)
        {
            var promocion = new Promocion(
                nombre,
                "Promoción de prueba por porcentaje.",
                TipoPromocion.Porcentaje,
                null,
                null,
                porcentaje,
                desde ?? new DateTime(2020,1,1),
                hasta ?? new DateTime(2099,12,31),
                EnumerableExtensions.DiasTodos);

            if (!activa)
                promocion.Desactivar();

            Context.Promociones.Add(promocion);

            await Context.SaveChangesAsync();

            return promocion;
        }
    }

    internal static class EnumerableExtensions
    {
        public static IReadOnlyCollection<DayOfWeek> DiasTodos { get; } =
        [
            DayOfWeek.Sunday,
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday
        ];
    }
}
