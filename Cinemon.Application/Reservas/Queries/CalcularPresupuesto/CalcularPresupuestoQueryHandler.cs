using Cinemon.Application.Abstractions;
using Cinemon.Domain.Exceptions;
using Cinemon.Domain.Promociones;
using MediatR;
using System.Threading.Tasks;

namespace Cinemon.Application.Reservas.Queries.CalcularPresupuesto
{
    public sealed class CalcularPresupuestoQueryHandler: IRequestHandler<CalcularPresupuestoQuery,PresupuestoDto>
    {
        private readonly IFuncionRepository _funcionRepository;
        private readonly IPromocionRepository _promocionRepository;

        public CalcularPresupuestoQueryHandler(
            IFuncionRepository funcionRepository,
            IPromocionRepository promocionRepository)
        {
            _funcionRepository = funcionRepository;
            _promocionRepository = promocionRepository;
        }

        public async Task<PresupuestoDto> Handle(CalcularPresupuestoQuery request,
            CancellationToken cancellationToken)
        {
            if (request.CantidadButacas <= 0) {
                throw new BusinessRuleException(
                    "Seleccione al menos una butaca.");
            }

            var funcion = await _funcionRepository.ObtenerPorIdAsync(
                request.FuncionId,
                cancellationToken);

            if (funcion is null) {
                throw new NotFoundException(
                    "La funcion no existe.");
            }

            var promociones = await _promocionRepository.ObtenerActivasParaFechaAsync(
                funcion.FechaHoraInicio,
                cancellationToken);

            var resultado = MotorPromociones.Calcular(
                promociones,
                funcion.FechaHoraInicio,
                funcion.Precio,
                request.CantidadButacas);

            return new PresupuestoDto(
                funcion.Id,
                funcion.Precio,
                request.CantidadButacas,
                resultado.EntradasAPagar,
                resultado.Promocion?.Id,
                resultado.Promocion?.Nombre,
                resultado.Subtotal,
                resultado.Total,
                resultado.Ahorro);
        }
    }
}
