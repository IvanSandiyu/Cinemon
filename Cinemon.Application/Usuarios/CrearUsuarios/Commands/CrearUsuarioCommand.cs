using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.CrearUsuarios.Commands
{
    public sealed record CrearUsuarioCommand(
     string NombreApellido,
     string Email,
     string Password): IRequest<int>;
}
