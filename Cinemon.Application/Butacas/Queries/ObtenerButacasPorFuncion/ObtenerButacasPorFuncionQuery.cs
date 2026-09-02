using Cinemon.Application.Features.Butacas.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Butacas.Queries.ObtenerButacasPorFuncion
{
    public sealed record ObtenerButacasPorFuncionQuery(int FuncionId) : IRequest<IReadOnlyCollection<ButacaFuncionDto>>;
}
