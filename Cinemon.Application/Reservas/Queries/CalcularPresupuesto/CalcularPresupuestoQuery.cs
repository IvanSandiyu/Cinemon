using MediatR;

namespace Cinemon.Application.Reservas.Queries.CalcularPresupuesto
{
    public sealed record CalcularPresupuestoQuery(
     int FuncionId,
     int CantidadButacas) : IRequest<PresupuestoDto>;

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
