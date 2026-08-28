using MediatR;

namespace Cinemon.Application.Reservas.Commands.CancelarReserva
{
    public sealed record CancelarReservaCommand(int Id) : IRequest<bool>;
}
