using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Common
{
    public static class RelojCine
    {
        private static readonly TimeZoneInfo Zona = TimeZoneInfo.FindSystemTimeZoneById(
            OperatingSystem.IsWindows() ? "Argentina Standard Time" : "America/Argentina/Buenos_Aires");

        public static DateTime Ahora(this TimeProvider reloj) =>
            TimeZoneInfo.ConvertTime(reloj.GetUtcNow(), Zona).DateTime;
    }
}
