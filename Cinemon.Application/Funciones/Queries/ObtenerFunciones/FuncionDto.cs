using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Queries.ObtenerFunciones
{
    public sealed record FuncionDto
    ( 
        int peliculaId,
        int salaId,
        DateTime fechaHoraInicio,
        IdiomaFuncion idioma,
        Formato formato,
        decimal precio,
        IReadOnlyCollection<string> Funciones);
}
