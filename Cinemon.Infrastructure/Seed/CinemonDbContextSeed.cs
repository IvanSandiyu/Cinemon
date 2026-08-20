using Cinemon.Domain.Entidades.Butacas;
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
    }
}
