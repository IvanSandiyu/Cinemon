using Cinemon.Application.Abstractions;
using MediatR;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed class ObtenerReservasQueryHandler: IRequestHandler<ObtenerReservasQuery, IReadOnlyCollection<ReservaDto>>
    {
        private readonly IReservaRepository _reservaRepository;

        public ObtenerReservasQueryHandler(IReservaRepository reservaRepository)
        {
            _reservaRepository = reservaRepository;
        }

        public Task<IReadOnlyCollection<ReservaDto>> Handle(ObtenerReservasQuery request,CancellationToken cancellationToken)
        {
            return _reservaRepository.ObtenerReservasAsync(usuarioId: null, cancellationToken);
        }
    }
}

