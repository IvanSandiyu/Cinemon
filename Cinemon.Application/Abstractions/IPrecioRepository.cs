using Cinemon.Domain.Entidades.Precios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IPrecioRepository
    {
        Task<IReadOnlyCollection<Precio>> ObtenerTodosAsync(
            CancellationToken cancellationToken);

        Task<Precio?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken);

        Task GuardarAsync(
            CancellationToken cancellationToken);
    }
}
