using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Reservas.Commands.CrearReserva
{
    public sealed record CrearReservaCommand(
    int FuncionId,
    IReadOnlyCollection<int> ButacasIds) : IRequest<int>;
}
