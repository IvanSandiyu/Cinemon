using Cinemon.Application.Abstractions;
using Cinemon.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.FinalizarFuncionVencida
{
    public sealed class FinalizarFuncionesVencidasCommandHandler
       : IRequestHandler<FinalizarFuncionesVencidasCommand, int>
    {
        private readonly IFuncionRepository _funcionRepository;
        private readonly TimeProvider _timeProvider;

        public FinalizarFuncionesVencidasCommandHandler(IFuncionRepository funcionRepository,
            TimeProvider timeProvider)
        {
            _funcionRepository = funcionRepository;
            _timeProvider = timeProvider;
        }

        public Task<int> Handle(FinalizarFuncionesVencidasCommand request,CancellationToken cancellationToken)
        {
            return _funcionRepository.FinalizarVencidasAsync(
                _timeProvider.Ahora(),
                cancellationToken);
        }
    }
}
