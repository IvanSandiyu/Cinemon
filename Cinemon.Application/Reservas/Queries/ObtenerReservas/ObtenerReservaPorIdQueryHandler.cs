using Cinemon.Application.Abstractions;
using MediatR;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed class ObtenerReservaPorIdQueryHandler
        : IRequestHandler<ObtenerReservaPorIdQuery, ReservaDto?>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IFuncionRepository _funcionRepository;
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly ISalaRepository _salaRepository;
        private readonly IButacaRepository _butacaRepository;

        public ObtenerReservaPorIdQueryHandler(
            IReservaRepository reservaRepository,
            IUsuarioRepository usuarioRepository,
            IFuncionRepository funcionRepository,
            IPeliculaRepository peliculaRepository,
            ISalaRepository salaRepository,
            IButacaRepository butacaRepository)
        {
            _reservaRepository = reservaRepository;
            _usuarioRepository = usuarioRepository;
            _funcionRepository = funcionRepository;
            _peliculaRepository = peliculaRepository;
            _salaRepository = salaRepository;
            _butacaRepository = butacaRepository;
        }

        public async Task<ReservaDto?> Handle(
            ObtenerReservaPorIdQuery request,
            CancellationToken cancellationToken)
        {
            var reserva = await _reservaRepository
                .ObtenerPorIdAsync(request.Id, cancellationToken);

            if (reserva is null)
                return null;

            return await ReservaMapper.ToDtoAsync(
                reserva,
                _usuarioRepository,
                _funcionRepository,
                _peliculaRepository,
                _salaRepository,
                _butacaRepository,
                _reservaRepository,
                cancellationToken);
        }
    }
}
