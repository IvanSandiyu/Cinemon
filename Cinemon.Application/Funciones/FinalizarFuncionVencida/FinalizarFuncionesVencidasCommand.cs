using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.FinalizarFuncionVencida
{
    // Devuelve la cantidad de funciones que se marcaron como Finalizadas.
    public sealed record FinalizarFuncionesVencidasCommand : IRequest<int>;
    
}
