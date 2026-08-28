using MediatR;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed record ObtenerReservaPorIdQuery(int Id) : IRequest<ReservaDto?>;
}
