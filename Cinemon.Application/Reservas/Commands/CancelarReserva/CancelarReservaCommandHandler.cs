using Cinemon.Application.Abstractions;
using MediatR;

namespace Cinemon.Application.Reservas.Commands.CancelarReserva
{
    public sealed class CancelarReservaCommandHandler
        : IRequestHandler<CancelarReservaCommand, bool>
    {
        private readonly IReservaRepository _reservaRepository;

        public CancelarReservaCommandHandler(
            IReservaRepository reservaRepository)
        {
            _reservaRepository = reservaRepository;
        }

        public async Task<bool> Handle(
            CancelarReservaCommand request,
            CancellationToken cancellationToken)
        {
            return await _reservaRepository.CancelarAsync(
                request.Id,
                cancellationToken);
        }
    }
}
