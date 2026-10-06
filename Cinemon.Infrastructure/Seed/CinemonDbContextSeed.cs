using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Entidades.Generos;
using Cinemon.Domain.Entidades.Precios;
using Cinemon.Domain.Entidades.Promociones;
using Cinemon.Domain.Entidades.Salas;
using Cinemon.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.Seed
{
    public static class CinemonDbContextSeed
    {
        public static async Task SeedAsync(CinemonDbContext context)
        {
            await context.Database.MigrateAsync();
            //await SeedSalasAsync(context);
            await SeedGenerosAsync(context);
            await SeedPreciosAsync(context);
            await SeedPromocionesAsync(context);

            if (await context.Salas.AnyAsync())
                return;

            var salas = new List<Sala>
        {
            new(1, TipoSala.Estandar),
            new(2, TipoSala.Estandar),
            new(3, TipoSala.Estandar),
            new(4, TipoSala.Imax)
        };

            context.Salas.AddRange(salas);

            await context.SaveChangesAsync();

            var butacas = new List<Butaca>();

            GenerarButacas(
                butacas,
                salas[0].Id,
                cantidadFilas: 8,
                butacasPorFila: 10);

            GenerarButacas(
                butacas,
                salas[1].Id,
                cantidadFilas: 10,
                butacasPorFila: 12);

            GenerarButacas(
                butacas,
                salas[2].Id,
                cantidadFilas: 10,
                butacasPorFila: 15);

            GenerarButacas(
                butacas,
                salas[3].Id,
                cantidadFilas: 11,
                butacasPorFila: 20);

            context.Butacas.AddRange(butacas);

            await context.SaveChangesAsync();
        }


        private static void GenerarButacas(
            ICollection<Butaca> butacas,
            int salaId,
            int cantidadFilas,
            int butacasPorFila)
        {
            for (var filaIndex = 0; filaIndex < cantidadFilas; filaIndex++) {
                var fila = ((char)('A' + filaIndex)).ToString();

                for (var numero = 1; numero <= butacasPorFila; numero++) {
                    butacas.Add(
                        new Butaca(
                            salaId,
                            fila,
                            numero));
                }
            }
        }

        private static async Task SeedPreciosAsync(CinemonDbContext context)
        {
            if (await context.Precios.AnyAsync())
                return;

            var precios = new List<Precio>
        {
            new(Formato.DosD, TipoSala.Estandar, 8000m),
            new(Formato.TresD, TipoSala.Estandar, 10000m),
            new(Formato.CuatroD, TipoSala.Estandar, 14000m),
            new(Formato.DosD, TipoSala.Imax, 12000m)
        };

            context.Precios.AddRange(precios);

            await context.SaveChangesAsync();
        }

        private static async Task SeedPromocionesAsync(CinemonDbContext context)
        {
            if (await context.Promociones.AnyAsync())
                return;

            var hoy = DateTime.UtcNow.Date;

            var promociones = new List<Promocion>
        {
            new(
                "2x1 Lunes a Miércoles",
                "Se cobran 2 entradas y la tercera es gratis. Válida en funciones de lunes, martes y miércoles.",
                TipoPromocion.NxM,
                2,
                1,
                null,
                hoy,
                hoy.AddYears(5),
                [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday]),
            new(
                "20% OFF enTodos los días",
                "Descuento del 20% sobre el subtotal de la reserva.",
                TipoPromocion.Porcentaje,
                null,
                null,
                20m,
                hoy,
                hoy.AddYears(5),
                [
                    DayOfWeek.Sunday,
                    DayOfWeek.Monday,
                    DayOfWeek.Tuesday,
                    DayOfWeek.Wednesday,
                    DayOfWeek.Thursday,
                    DayOfWeek.Friday,
                    DayOfWeek.Saturday
                ])
        };

            context.Promociones.AddRange(promociones);

            await context.SaveChangesAsync();
        }

        private static async Task SeedGenerosAsync(CinemonDbContext context)
        {
            if (await context.Generos.AnyAsync())
                return;

            var generos = new List<Genero> {
        new() { Nombre = "Acción" },
        new() { Nombre = "Comedia" },
        new() { Nombre = "Drama" },
        new() { Nombre = "Ciencia Ficción" },
        new() { Nombre = "Terror" }
    };

            context.Generos.AddRange(generos);

            await context.SaveChangesAsync();
        }

    }
}
