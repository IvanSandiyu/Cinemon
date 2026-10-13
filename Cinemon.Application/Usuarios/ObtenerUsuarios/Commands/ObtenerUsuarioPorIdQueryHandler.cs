using Cinemon.Application.Abstractions;
using Cinemon.Application.Interfaces;
using Cinemon.Application.Usuarios.DTOs;
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
    public sealed class ObtenerUsuarioPorIdQueryHandler : IRequestHandler<ObtenerUsuarioPorIdQuery, UsuarioDto?>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ICurrentUserService _currentUserService;

        public ObtenerUsuarioPorIdQueryHandler(IUsuarioRepository usuarioRepository,ICurrentUserService currentUserService)
        {
            _usuarioRepository = usuarioRepository;
            _currentUserService = currentUserService;
        }

        public async Task<UsuarioDto?> Handle(ObtenerUsuarioPorIdQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.Role != "Admin" &&
                request.Id != _currentUserService.UserId)
                return null;

            var usuario = await _usuarioRepository.ObtenerPorIdAsync(request.Id, cancellationToken);

            return usuario is null
                ? null
                : new UsuarioDto(usuario.Id, usuario.NombreApellido, usuario.Email,
                                 usuario.Activo, usuario.Rol.ToString());
        }
    }
}
