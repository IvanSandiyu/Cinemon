using Cinemon.Application.Abstractions;
using Cinemon.Application.Promociones.Commands.CrearPromocion;
using Cinemon.Domain.Entidades.Promociones;
using MediatR;

namespace Cinemon.Application.Promociones.Commands.CrearPromocion
{
    public sealed class CrearPromocionCommandHandler
    : IRequestHandler<CrearPromocionCommand,int>
    {
        private readonly IPromocionRepository _promocionRepository;

        public CrearPromocionCommandHandler(IPromocionRepository promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        public async Task<int> Handle(CrearPromocionCommand request,CancellationToken cancellationToken)
        {
            var promocion = new Promocion(
                request.Nombre,
                request.Descripcion,
                request.Tipo,
                request.CantidadPagadas,
                request.CantidadGratis,
                request.PorcentajeDescuento,
                request.FechaDesde,
                request.FechaHasta,
                request.DiasSemana);

            await _promocionRepository.AddAsync(promocion,cancellationToken);

            return promocion.Id;
        }
    }
}
