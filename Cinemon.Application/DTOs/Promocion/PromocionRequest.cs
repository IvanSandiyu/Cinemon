using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Cinemon.Application.DTOs.Promocion
{
    public sealed record PromocionDto(
     int Id,
     string Nombre,
     string? Descripcion,
     int Tipo,
     int? CantidadPagadas,
     int? CantidadGratis,
     decimal? PorcentajeDescuento,
     DateTime FechaDesde,
     DateTime FechaHasta,
     bool Activa,
     IReadOnlyCollection<int> DiasSemana);

    public sealed record CrearPromocionRequest(
     string Nombre,
     string? Descripcion,
     TipoPromocion Tipo,
     int? CantidadPagadas,
     int? CantidadGratis,
     decimal? PorcentajeDescuento,
     DateTime FechaDesde,
     DateTime FechaHasta,
     IReadOnlyCollection<DayOfWeek> DiasSemana);

    public sealed record ActualizarPromocionRequest(
     int Id,
     string Nombre,
     string? Descripcion,
     TipoPromocion Tipo,
     int? CantidadPagadas,
     int? CantidadGratis,
     decimal? PorcentajeDescuento,
     DateTime FechaDesde,
     DateTime FechaHasta,
     IReadOnlyCollection<DayOfWeek> DiasSemana);
}
