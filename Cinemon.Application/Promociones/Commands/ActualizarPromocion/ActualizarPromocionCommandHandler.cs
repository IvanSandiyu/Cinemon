using Cinemon.Application.Abstractions;
using Cinemon.Application.Promociones.Commands.ActualizarPromocion;
using Cinemon.Domain.Exceptions;
using MediatR;

namespace Cinemon.Application.Promociones.Commands.ActualizarPromocion
{
    public sealed class ActualizarPromocionCommandHandler
    : IRequestHandler<ActualizarPromocionCommand,Unit>
    {
        private readonly IPromocionRepository _promocionRepository;

        public ActualizarPromocionCommandHandler(IPromocionRepository promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        public async Task<Unit> Handle(ActualizarPromocionCommand request,CancellationToken cancellationToken)
        {
            var promocion = await _promocionRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (promocion is null) {
                throw new NotFoundException(
                    "La promoción no existe.");
            }

            promocion.Actualizar(
                request.Nombre,
                request.Descripcion,
                request.Tipo,
                request.CantidadPagadas,
                request.CantidadGratis,
                request.PorcentajeDescuento,
                request.FechaDesde,
                request.FechaHasta,
                request.DiasSemana);

            _promocionRepository.Update(promocion);

            await _promocionRepository.GuardarAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
