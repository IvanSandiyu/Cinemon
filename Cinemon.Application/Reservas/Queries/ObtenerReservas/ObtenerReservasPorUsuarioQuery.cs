using MediatR;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed record ObtenerReservasPorUsuarioQuery(
        int UsuarioId) : IRequest<IReadOnlyCollection<ReservaDto>>;
}