using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Precios.Commands.ActualizarPrecio
{
    public sealed record ActualizarPrecioCommand(
     int Id,
     decimal Valor) : IRequest<Unit>;
}
