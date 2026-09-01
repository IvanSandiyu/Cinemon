using Cinemon.Domain.Entidades.Usuarios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.ObtenerUsuarios.Queries
{
    public sealed record ObtenerUsuarioPorIdQuery(int Id) : IRequest<Usuario?>;
}
