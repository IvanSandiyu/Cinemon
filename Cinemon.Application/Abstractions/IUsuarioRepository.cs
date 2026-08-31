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
        Task AddAsync(Usuario usuario, CancellationToken cancellationToken);
        Task<Usuario?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken);

        Task<bool> ExistePorEmailAsync(string email,CancellationToken cancellationToken);

        Task<Usuario?> ObtenerPorEmailAsync(string email,CancellationToken cancellationToken);
    }
}
