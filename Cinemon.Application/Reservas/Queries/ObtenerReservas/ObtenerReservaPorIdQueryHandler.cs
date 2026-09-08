using Cinemon.Application.Abstractions;
using Cinemon.Application.Interfaces;
using MediatR;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed class ObtenerReservaPorIdQueryHandler: IRequestHandler<ObtenerReservaPorIdQuery, ReservaDto?>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly ICurrentUserService _currentUserService;

        public ObtenerReservaPorIdQueryHandler(IReservaRepository reservaRepository,ICurrentUserService currentUserService)
        {
            _reservaRepository = reservaRepository;
            _currentUserService = currentUserService;
        }

        public async Task<ReservaDto?> Handle(ObtenerReservaPorIdQuery request,CancellationToken cancellationToken)
        {
            // Consulta liviana (una sola fila) solo para validar quién es el dueño
            var reserva = await _reservaRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (reserva is null)
                return null;

            if (_currentUserService.Role != "Admin" &&
                reserva.UsuarioId != _currentUserService.UserId) {
                return null;
            }

            return await _reservaRepository.ObtenerDetalleAsync(
                request.Id,
                cancellationToken);
        }
    }
}
