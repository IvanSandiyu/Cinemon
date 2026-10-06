using Cinemon.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;

namespace Cinemon.Application.Promociones.Commands.CrearPromocion
{
    public sealed record CrearPromocionCommand(
     string Nombre,
     string? Descripcion,
     TipoPromocion Tipo,
     int? CantidadPagadas,
     int? CantidadGratis,
     decimal? PorcentajeDescuento,
     DateTime FechaDesde,
     DateTime FechaHasta,
     IReadOnlyCollection<DayOfWeek> DiasSemana) : IRequest<int>;
}
