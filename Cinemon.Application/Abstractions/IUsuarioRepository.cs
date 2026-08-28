using Cinemon.Domain.Entidades.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken);
    }
}
