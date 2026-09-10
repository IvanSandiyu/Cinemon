using Cinemon.Application.Abstractions;
using Cinemon.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.ActivarUsuarios.Commands
{
    public sealed class DesactivarUsuarioCommandHandler: IRequestHandler<DesactivarUsuarioCommand>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public DesactivarUsuarioCommandHandler(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task Handle(DesactivarUsuarioCommand request,CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (usuario is null)
                throw new NotFoundException(
                    "El usuario no existe.");

            usuario.Desactivar();

            await _usuarioRepository.UpdateAsync(
                usuario,
                cancellationToken);
        }
    }
}
