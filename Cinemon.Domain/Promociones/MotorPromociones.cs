using Cinemon.Domain.Entidades.Promociones;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Domain.Promociones
{
    /// <summary>
    /// Calcula el mejor descuento disponible entre todas las promociones
    /// aplicables a una función. Regla: gana la promoción que deja el
    /// menor total; si ninguna aplica, se cobra el subtotal completo.
    /// </summary>
    public static class MotorPromociones
    {
        public static ResultadoPromocion Calcular(
            IEnumerable<Promocion> promociones,
            DateTime fechaHoraFuncion,
            decimal precio,
            int entradas)
        {
            var subtotal = precio * entradas;

            if (entradas <= 0) {
                return new ResultadoPromocion(
                    null,
                    0,
                    0,
                    0m,
                    0m,
                    0m);
            }

            var mejor = promociones
                .Where(x => x.AplicaPara(fechaHoraFuncion))
                .Select(x => new
                {
                    Promocion = x,
                    Total = x.CalcularTotal(fechaHoraFuncion,precio,entradas)
                })
                .OrderBy(x => x.Total)
                .ThenByDescending(x => x.Promocion.Id)
                .FirstOrDefault();

            if (mejor is null) {
                return new ResultadoPromocion(
                    null,
                    entradas,
                    entradas,
                    subtotal,
                    subtotal,
                    0m);
            }

            return new ResultadoPromocion(
                mejor.Promocion,
                entradas,
                mejor.Promocion.CalcularEntradasAPagar(fechaHoraFuncion,entradas),
                subtotal,
                mejor.Total,
                subtotal - mejor.Total);
        }
    }

    /// <summary>
    /// Resultado del cálculo: qué promoción ganó (si alguna), cuántas
    /// entradas se cobran y el desglose de subtotal / ahorro / total.
    /// </summary>
    public sealed record ResultadoPromocion(
        Promocion? Promocion,
        int Entradas,
        int EntradasAPagar,
        decimal Subtotal,
        decimal Total,
        decimal Ahorro);
}
