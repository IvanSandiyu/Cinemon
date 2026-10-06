using Cinemon.Application.Abstractions;
using Cinemon.Application.DTOs.Promocion;
using Cinemon.Domain.Entidades.Promociones;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Application.Promociones.Queries
{
    public sealed record ObtenerPromocionesQuery: IRequest<IReadOnlyCollection<PromocionDto>>;
}
