using Cinemon.Application.Abstractions;
using Cinemon.Application.Precios.Commands.ActualizarPrecio;
using Cinemon.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Precios.Commands.ActualizarPrecio
{
    public sealed class ActualizarPrecioCommandHandler: IRequestHandler<ActualizarPrecioCommand,Unit>
    {
        private readonly IPrecioRepository _precioRepository;

        public ActualizarPrecioCommandHandler(IPrecioRepository precioRepository)
        {
            _precioRepository = precioRepository;
        }

        public async Task<Unit> Handle(ActualizarPrecioCommand request,CancellationToken cancellationToken)
        {
            var precio = await _precioRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (precio is null) {
                throw new NotFoundException(
                    "El precio no existe.");
            }

            precio.ActualizarValor(request.Valor);

            await _precioRepository.GuardarAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
