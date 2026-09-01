using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.FinalizarFuncion
{
    public sealed record FinalizarFuncionCommand(int Id) : IRequest;
}
