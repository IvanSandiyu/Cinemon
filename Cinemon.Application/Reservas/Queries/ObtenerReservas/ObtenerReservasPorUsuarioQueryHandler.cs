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

        public async Task<IReadOnlyCollection<ReservaDto>> Handle(
            ObtenerReservasPorUsuarioQuery request,
            CancellationToken cancellationToken)
        {
            var reservas = await _reservaRepository
                .ObtenerPorUsuarioAsync(request.UsuarioId, cancellationToken);

            var dto = new List<ReservaDto>();

            foreach (var reserva in reservas)
            {
                dto.Add(await ReservaMapper.ToDtoAsync(
                    reserva,
                    _usuarioRepository,
                    _funcionRepository,
                    _peliculaRepository,
                    _salaRepository,
                    _butacaRepository,
                    _reservaRepository,
                    cancellationToken));
            }

            return dto;
        }
    }
}