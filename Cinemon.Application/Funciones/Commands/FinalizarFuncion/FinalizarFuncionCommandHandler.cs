using Cinemon.Application.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.FinalizarFuncion
{
    public sealed class FinalizarFuncionCommandHandler: IRequestHandler<FinalizarFuncionCommand>
    {
        private readonly IFuncionRepository _funcionRepository;

        public FinalizarFuncionCommandHandler(IFuncionRepository funcionRepository)
        {
            _funcionRepository = funcionRepository;
        }

        public async Task Handle(FinalizarFuncionCommand request,CancellationToken cancellationToken)
        {
            var funcion = await _funcionRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (funcion is null)
                throw new KeyNotFoundException(
                    "La función no existe.");

            funcion.Finalizar();

            await _funcionRepository.UpdateAsync(
                funcion,
                cancellationToken);
        }
    }
}
