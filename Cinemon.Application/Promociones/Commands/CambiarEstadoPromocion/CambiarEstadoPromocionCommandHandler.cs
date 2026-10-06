using Cinemon.Application.Abstractions;
using Cinemon.Application.Promociones.Commands.CambiarEstadoPromocion;
using Cinemon.Domain.Exceptions;
using MediatR;

namespace Cinemon.Application.Promociones.Commands.CambiarEstadoPromocion
{
    public sealed class CambiarEstadoPromocionCommandHandler
    : IRequestHandler<CambiarEstadoPromocionCommand,Unit>
    {
        private readonly IPromocionRepository _promocionRepository;

        public CambiarEstadoPromocionCommandHandler(IPromocionRepository promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        public async Task<Unit> Handle(CambiarEstadoPromocionCommand request,CancellationToken cancellationToken)
        {
            var promocion = await _promocionRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (promocion is null) {
                throw new NotFoundException(
                    "La promoción no existe.");
            }

            if (request.Activa)
                promocion.Activar();
            else
                promocion.Desactivar();

            _promocionRepository.Update(promocion);

            await _promocionRepository.GuardarAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
