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
    public sealed class ObtenerUsuarioPorIdQueryHandler: IRequestHandler<ObtenerUsuarioPorIdQuery,Usuario?>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ObtenerUsuarioPorIdQueryHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> Handle(ObtenerUsuarioPorIdQuery request,CancellationToken cancellationToken)
        {
            return await _usuarioRepository.ObtenerPorIdAsync(request.Id,cancellationToken);
        }
    }
}
