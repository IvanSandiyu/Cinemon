using Cinemon.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.DTOs
{
    public sealed record LoginDto(
      string Token,
      int UsuarioId,
      string NombreApellido,
      Rol Rol);
}
