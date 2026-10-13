using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.DTOs
{
    public sealed record UsuarioDto(
        int Id,
        string NombreApellido,
        string Email,
        bool Activo,
        string Rol);
}
