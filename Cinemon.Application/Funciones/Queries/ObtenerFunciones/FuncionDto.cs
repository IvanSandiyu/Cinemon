using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Queries.ObtenerFunciones
{
    public sealed record FuncionDto(
    int Id,
    int PeliculaId,
    int SalaId,
    DateTime FechaHoraInicio,
    IdiomaFuncion Idioma,
    Formato Formato,
    decimal Precio,
    EstadoFuncion EstadoFuncion);
}
