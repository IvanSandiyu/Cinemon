namespace Cinemon.Web.Models.DTOs
{
    public sealed record PresupuestoDto(
        int FuncionId,
        decimal PrecioEntrada,
        int CantidadEntradas,
        int EntradasAPagar,
        bool Aplica2x1,
        decimal Subtotal,
        decimal Total,
        decimal Ahorro);
}
