using Cinemon.Application.Abstractions;
using MediatR;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed class ObtenerReservasPorUsuarioQueryHandler
        : IRequestHandler<ObtenerReservasPorUsuarioQuery, IReadOnlyCollection<ReservaDto>>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IFuncionRepository _funcionRepository;
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly ISalaRepository _salaRepository;
        private readonly IButacaRepository _butacaRepository;

        public ObtenerReservasPorUsuarioQueryHandler(
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

        public Task<IReadOnlyCollection<ReservaDto>> Handle(ObtenerReservasPorUsuarioQuery request,CancellationToken cancellationToken)
        {
            return _reservaRepository.ObtenerReservasAsync(request.UsuarioId, cancellationToken);
        }
    }
}