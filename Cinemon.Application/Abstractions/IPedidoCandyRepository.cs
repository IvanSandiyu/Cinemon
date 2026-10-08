using Cinemon.Domain.Entidades.Candy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IPedidoCandyRepository
    {
        Task AgregarAsync(
            PedidoCandy pedido,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<PedidoCandy>> ObtenerPorUsuarioAsync(
            int usuarioId,
            CancellationToken cancellationToken);
    }
}
