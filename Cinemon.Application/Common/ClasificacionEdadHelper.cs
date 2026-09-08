using Cinemon.Domain.Enums;

namespace Cinemon.Application.Common
{
    public static class ClasificacionEdadHelper
    {
        public static string ParaMostrar(ClasificacionEdad clasificacion)
        {
            return clasificacion switch
            {
                ClasificacionEdad.ATP => "ATP",
                ClasificacionEdad.MayorDe13 => "+13",
                ClasificacionEdad.MayorDe16 => "+16",
                ClasificacionEdad.MayorDe18 => "+18",
                _ => clasificacion.ToString()
            };
        }
    }
}