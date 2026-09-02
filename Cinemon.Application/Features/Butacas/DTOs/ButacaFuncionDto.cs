using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Features.Butacas.DTOs
{
    public sealed record ButacaFuncionDto(
    int Id,
    string Fila,
    int Numero,
    bool Ocupada);
}
