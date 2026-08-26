using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Queries.ObtenerFunciones
{
    public sealed record ObtenerFuncionesQuery: IRequest<IReadOnlyCollection<FuncionDto>>;
}
