using Cinemon.Application.Abstractions;
using Cinemon.Application.Common;
using Cinemon.Domain.Exceptions;
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
        private readonly TimeProvider _timeProvider;

        public FinalizarFuncionCommandHandler(
            IFuncionRepository funcionRepository,
            TimeProvider timeProvider)
        {
            _funcionRepository = funcionRepository;
            _timeProvider = timeProvider;
        }

        public async Task Handle(FinalizarFuncionCommand request,CancellationToken cancellationToken)
        {
            var funcion = await _funcionRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (funcion is null)
                throw new NotFoundException(
                    "La función no existe.");

            funcion.Finalizar(_timeProvider.Ahora());

            await _funcionRepository.UpdateAsync(
                funcion,
                cancellationToken);
        }
    }
}
