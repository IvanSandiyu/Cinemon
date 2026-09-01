using Cinemon.Application.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.ActivarUsuarios.Commands
{
    public sealed class ActivarUsuarioCommandHandler: IRequestHandler<ActivarUsuarioCommand>
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public ActivarUsuarioCommandHandler(
            IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task Handle(
            ActivarUsuarioCommand request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (usuario is null)
                throw new KeyNotFoundException(
                    "El usuario no existe.");

            usuario.Activar();

            await _usuarioRepository.UpdateAsync(
                usuario,
                cancellationToken);
        }
    }
}
