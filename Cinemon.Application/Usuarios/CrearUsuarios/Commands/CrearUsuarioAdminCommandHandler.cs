using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Usuarios;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.CrearUsuarios.Commands
{
    public sealed class CrearUsuarioAdminCommandHandler: IRequestHandler<CrearUsuarioAdminCommand, int>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordService _passwordService;

        public CrearUsuarioAdminCommandHandler(IUsuarioRepository usuarioRepository,IPasswordService passwordService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordService = passwordService;
        }

        public async Task<int> Handle(CrearUsuarioAdminCommand request,CancellationToken cancellationToken)
        {
            var emailExiste = await _usuarioRepository.ExistePorEmailAsync(
                request.Email,
                cancellationToken);

            if (emailExiste) {
                throw new InvalidOperationException(
                    "Ya existe un usuario registrado con ese email.");
            }

            var passwordHash = _passwordService.HashPassword(
                request.Password);

            var usuario = new Usuario(
                request.NombreApellido,
                request.Email,
                request.Rol,
                passwordHash);

            await _usuarioRepository.AddAsync(
                usuario,
                cancellationToken);

            return usuario.Id;
        }
    }
}
