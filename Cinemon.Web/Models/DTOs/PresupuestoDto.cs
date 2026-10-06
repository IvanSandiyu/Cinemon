namespace Cinemon.Web.Models.DTOs
{
    public sealed record PresupuestoDto(
        int FuncionId,
        decimal PrecioEntrada,
        int CantidadEntradas,
        int EntradasAPagar,
        int? PromocionId,
        string? PromocionNombre,
        decimal Subtotal,
        decimal Total,
        decimal Ahorro)
    {
        public bool AplicaPromocion => PromocionId is not null;
    }
}
