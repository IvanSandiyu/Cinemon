using Cinemon.Application.Abstractions;
using Cinemon.Application.Promociones.Commands.EliminarPromocion;
using Cinemon.Domain.Exceptions;
using MediatR;

namespace Cinemon.Application.Promociones.Commands.EliminarPromocion
{
    public sealed class EliminarPromocionCommandHandler
    : IRequestHandler<EliminarPromocionCommand,Unit>
    {
        private readonly IPromocionRepository _promocionRepository;

        public EliminarPromocionCommandHandler(IPromocionRepository promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        public async Task<Unit> Handle(EliminarPromocionCommand request,CancellationToken cancellationToken)
        {
            var promocion = await _promocionRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (promocion is null) {
                throw new NotFoundException(
                    "La promoción no existe.");
            }

            await _promocionRepository.EliminarAsync(request.Id,cancellationToken);

            return Unit.Value;
        }
    }
}
