using Cinemon.Application.Abstractions;
using Cinemon.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.CancelarFuncion
{
    public sealed class CancelarFuncionCommandHandler: IRequestHandler<CancelarFuncionCommand>
    {
        private readonly IFuncionRepository _funcionRepository;

        public CancelarFuncionCommandHandler(IFuncionRepository funcionRepository)
        {
            _funcionRepository = funcionRepository;
        }

        public async Task Handle(CancelarFuncionCommand request,CancellationToken cancellationToken)
        {
            var funcion = await _funcionRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (funcion is null)
                throw new NotFoundException(
                    "La función no existe.");

            funcion.Cancelar();

            await _funcionRepository.UpdateAsync(
                funcion,
                cancellationToken);
        }
    }
}
