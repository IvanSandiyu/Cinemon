using Cinemon.Application.Usuarios.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.Login
{
    public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<LoginDto>;
}
