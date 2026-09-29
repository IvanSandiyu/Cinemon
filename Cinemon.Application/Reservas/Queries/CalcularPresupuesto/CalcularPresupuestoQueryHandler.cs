using Cinemon.Application.Abstractions;
using Cinemon.Application.Reservas.Queries.CalcularPresupuesto;
using Cinemon.Domain.Exceptions;
using Cinemon.Domain.Promociones;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Reservas.Queries.CalcularPresupuesto
{
    public sealed class CalcularPresupuestoQueryHandler: IRequestHandler<CalcularPresupuestoQuery,PresupuestoDto>
    {
        private readonly IFuncionRepository _funcionRepository;

        public CalcularPresupuestoQueryHandler(IFuncionRepository funcionRepository)
        {
            _funcionRepository = funcionRepository;
        }

        public async Task<PresupuestoDto> Handle(CalcularPresupuestoQuery request,
            CancellationToken cancellationToken)
        {
            if (request.CantidadButacas <= 0) {
                throw new BusinessRuleException(
                    "Seleccioná al menos una butaca.");
            }

            var funcion = await _funcionRepository.ObtenerPorIdAsync(
                request.FuncionId,
                cancellationToken);

            if (funcion is null) {
                throw new NotFoundException(
                    "La función no existe.");
            }

            var subtotal = funcion.Precio * request.CantidadButacas;

            var entradasAPagar = Promocion2x1.CalcularEntradasAPagar(
                funcion.FechaHoraInicio,
                request.CantidadButacas);

            var total = funcion.Precio * entradasAPagar;

            return new PresupuestoDto(
                funcion.Id,
                funcion.Precio,
                request.CantidadButacas,
                entradasAPagar,
                Promocion2x1.Aplica(funcion.FechaHoraInicio),
                subtotal,
                total,
                subtotal - total);
        }
    }
}
