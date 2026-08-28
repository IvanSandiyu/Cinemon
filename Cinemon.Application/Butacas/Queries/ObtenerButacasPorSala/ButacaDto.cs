using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Butacas.Queries.ObtenerButacasPorSala
{
    public sealed record ButacaDto(
    int Id,
    //int SalaId,
    string Fila,
    int Numero);
}
