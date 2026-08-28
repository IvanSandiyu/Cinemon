using Cinemon.Domain.Entidades.Butacas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IButacaRepository
    {
        Task<IReadOnlyCollection<Butaca>> ObtenerPorSalaAsync(int salaId,CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Butaca>> ObtenerPorIdsAsync(IReadOnlyCollection<int> ids,CancellationToken cancellationToken);
    }
}
