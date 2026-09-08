using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Usuarios;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.CrearUsuarios.Commands
{
    public sealed class CrearUsuarioCommandHandler: IRequestHandler<CrearUsuarioCommand, int>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordService _passwordService;

        public CrearUsuarioCommandHandler(IUsuarioRepository usuarioRepository,IPasswordService passwordService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordService = passwordService;
        }

        public async Task<int> Handle(CrearUsuarioCommand request,CancellationToken cancellationToken)
        {
            var emailExiste = await _usuarioRepository.ExistePorEmailAsync(
                request.Email,
                cancellationToken);

            if (emailExiste) {
                throw new ConflictException(
                    "Ya existe un usuario registrado con ese email.");
            }

            var passwordHash = _passwordService.HashPassword(
                request.Password);

            var usuario = new Usuario(
                request.NombreApellido,
                request.Email,
                Rol.Cliente,
                passwordHash);

            await _usuarioRepository.AddAsync(usuario,cancellationToken);

            return usuario.Id;
        }
    }
}
