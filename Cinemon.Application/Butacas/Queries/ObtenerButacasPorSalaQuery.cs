using Cinemon.Application.Butacas.Queries.ObtenerButacasPorSala;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Butacas.Queries
{
    public sealed record ObtenerButacasPorSalaQuery(int SalaId): IRequest<IReadOnlyCollection<ButacaDto>>;
}
