using Cinemon.Domain.Entidades.Salas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface ISalaRepository
    {
        Task<IReadOnlyCollection<Sala>> ObtenerTodasAsync(
            CancellationToken cancellationToken);

        Task<Sala?> ObtenerPorIdAsync(int id,CancellationToken cancellationToken);
    }
}
