using MediatR;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed record ObtenerReservasQuery() : IRequest<IReadOnlyCollection<ReservaDto>>;
}
