using Cinemon.Application.Abstractions;
using Cinemon.Application.Common;
using Cinemon.Application.Interfaces;
using Cinemon.Domain.Exceptions;
using MediatR;

namespace Cinemon.Application.Reservas.Commands.CancelarReserva
{
    public sealed class CancelarReservaCommandHandler: IRequestHandler<CancelarReservaCommand, bool>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFuncionRepository _funcionRepository;
        private readonly TimeProvider _timeProvider;

        public CancelarReservaCommandHandler(IReservaRepository reservaRepository,ICurrentUserService currentService, IFuncionRepository funcionRepository, TimeProvider timeProvider)
        {
            _reservaRepository = reservaRepository;
            _currentUserService = currentService;
            _funcionRepository = funcionRepository;
            _timeProvider = timeProvider;
        }

        public async Task<bool> Handle(CancelarReservaCommand request,CancellationToken cancellationToken)
        {
            var reserva = await _reservaRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (reserva is null)
                return false;

            var esAdmin = _currentUserService.Role == "Admin";

            if (!esAdmin && reserva.UsuarioId != _currentUserService.UserId)
                return false;

            var funcion = await _funcionRepository.ObtenerPorIdAsync(reserva.FuncionId, cancellationToken) 
                ?? throw new NotFoundException("Función", reserva.FuncionId);

            if (!esAdmin && funcion.FechaHoraInicio <= _timeProvider.Ahora())
                throw new BusinessRuleException("No se puede cancelar una reserva de una función que ya comenzó.");

            return await _reservaRepository.CancelarAsync(
                request.Id,
                cancellationToken);
        }
    }
}
