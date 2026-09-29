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
     bool Aplica2x1,
     decimal Subtotal,
     decimal Total,
     decimal Ahorro);
}
