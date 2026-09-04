using Cinemon.Application.DTOs.Genero;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Generos.Queries
{
    public sealed record ObtenerGenerosQuery: IRequest<IReadOnlyCollection<GeneroDto>>;
}
