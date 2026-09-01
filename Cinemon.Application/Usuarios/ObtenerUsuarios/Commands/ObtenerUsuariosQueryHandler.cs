using Cinemon.Application.Abstractions;
using Cinemon.Application.Usuarios.ObtenerUsuarios.Queries;
using Cinemon.Domain.Entidades.Usuarios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.ObtenerUsuarios.Commands
{
    public sealed class ObtenerUsuariosQueryHandler: IRequestHandler<ObtenerUsuariosQuery,IReadOnlyCollection<Usuario>>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerUsuariosQueryHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<IReadOnlyCollection<Usuario>> Handle(ObtenerUsuariosQuery request,CancellationToken cancellationToken)
        {
            return await _usuarioRepository.ObtenerTodosAsync(cancellationToken);
        }
    }
}
