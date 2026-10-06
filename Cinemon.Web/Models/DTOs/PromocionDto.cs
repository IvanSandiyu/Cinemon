namespace Cinemon.Web.Models.DTOs
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
        int Tipo,
        int? CantidadPagadas,
        int? CantidadGratis,
        decimal? PorcentajeDescuento,
        DateTime FechaDesde,
        DateTime FechaHasta,
        IReadOnlyCollection<int> DiasSemana);

    public sealed record ActualizarPromocionRequest(
        int Id,
        string Nombre,
        string? Descripcion,
        int Tipo,
        int? CantidadPagadas,
        int? CantidadGratis,
        decimal? PorcentajeDescuento,
        DateTime FechaDesde,
        DateTime FechaHasta,
        IReadOnlyCollection<int> DiasSemana);
}
