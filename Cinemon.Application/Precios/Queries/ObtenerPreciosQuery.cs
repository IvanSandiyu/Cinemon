using Cinemon.Application.DTOs.Precio;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Precios.Queries
{
    public sealed record ObtenerPreciosQuery: IRequest<IReadOnlyCollection<PrecioDto>>;
}
