using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.CancelarFuncion
{
    public sealed record CancelarFuncionCommand(
     int Id) : IRequest;
}
