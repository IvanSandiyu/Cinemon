using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Domain.Promociones
{
    /// <summary>
    /// Promoción 2x1 vigente de lunes a miércoles.
    /// Las entradas se cobran de a pares: una cantidad impar
    /// redondea hacia arriba (3 entradas pagan 2).
    /// </summary>
    public static class Promocion2x1
    {
        private static readonly DayOfWeek[] DiasConPromocion =
        [
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday
        ];

        public static bool Aplica(DateTime fechaHoraFuncion)
        {
            return DiasConPromocion.Contains(fechaHoraFuncion.DayOfWeek);
        }

        public static int CalcularEntradasAPagar(DateTime fechaHoraFuncion,int entradas)
        {
            if (entradas <= 0)
                return 0;

            return Aplica(fechaHoraFuncion)
                ? (int)Math.Ceiling(entradas / 2m)
                : entradas;
        }

        public static decimal CalcularTotal(DateTime fechaHoraFuncion,decimal precio,int entradas)
        {
            return precio * CalcularEntradasAPagar(fechaHoraFuncion,entradas);
        }
    }
}
