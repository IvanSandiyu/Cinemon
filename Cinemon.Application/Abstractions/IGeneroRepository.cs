using Cinemon.Domain.Entidades.Generos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IGeneroRepository
    {
        Task<bool> ExistAllAsync(IReadOnlyCollection<int> generoIds,CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Genero>> ObtenerTodosAsync(CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Genero>> ObtenerPorNombresAsync(IReadOnlyCollection<string> nombres, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Genero>> ObtenerOCrearPorNombresAsync(IReadOnlyCollection<string> nombres, CancellationToken cancellationToken);
    }
}
