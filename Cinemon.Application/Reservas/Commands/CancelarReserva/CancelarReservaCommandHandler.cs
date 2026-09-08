using Cinemon.Application.Abstractions;
using Cinemon.Application.Interfaces;
using MediatR;

namespace Cinemon.Application.Reservas.Commands.CancelarReserva
{
    public sealed class CancelarReservaCommandHandler: IRequestHandler<CancelarReservaCommand, bool>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly ICurrentUserService _currentUserService;

        public CancelarReservaCommandHandler(IReservaRepository reservaRepository,ICurrentUserService currentService)
        {
            _reservaRepository = reservaRepository;
            _currentUserService = currentService;
        }

        public async Task<bool> Handle(CancelarReservaCommand request,CancellationToken cancellationToken)
        {
            var reserva = await _reservaRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (reserva is null)
                return false;

            if (_currentUserService.Role != "Admin" &&
                reserva.UsuarioId != _currentUserService.UserId) {
                return false;
            }

            return await _reservaRepository.CancelarAsync(
                request.Id,
                cancellationToken);
        }
    }
}
