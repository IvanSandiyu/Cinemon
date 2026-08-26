using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Salas.Queries.ObtenerSalas
{
    public sealed record SalaDto(
    int Id,
    int Numero,
    string TipoSala,
    int CantidadButacas);
}
