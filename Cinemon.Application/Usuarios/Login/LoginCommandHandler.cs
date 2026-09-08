using Cinemon.Application.Abstractions;
using Cinemon.Application.Usuarios.DTOs;
using Cinemon.Application.Usuarios.Interfaces;
using Cinemon.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginDto>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IUsuarioRepository usuarioRepository, IPasswordService passwordService,
            ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _passwordService = passwordService;
            _tokenService = tokenService;
        }

        public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(request.Email, cancellationToken);

            if (usuario is null || !usuario.Activo)
                throw new InvalidCredentialsException();

            var passwordValida = _passwordService.VerifyPassword(request.Password, usuario.PasswordHash);

            if (!passwordValida)
                throw new InvalidCredentialsException();

            var token = _tokenService.GenerateToken(usuario);

            return new LoginDto(token, usuario.Id, usuario.NombreApellido, usuario.Rol);
        }
    }
}
