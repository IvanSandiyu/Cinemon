using Cinemon.Domain.Entidades.Candy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IProductoCandyRepository
    {
        Task<IReadOnlyCollection<ProductoCandy>> ObtenerTodasAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<ProductoCandy>> ObtenerActivasAsync(
            CancellationToken cancellationToken);

        Task<ProductoCandy?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<ProductoCandy>> ObtenerPorIdsAsync(
            IReadOnlyCollection<int> ids,
            CancellationToken cancellationToken);

        Task AddAsync(
            ProductoCandy producto,
            CancellationToken cancellationToken);

        void Update(ProductoCandy producto);

        Task<bool> EsComponenteDeAlgunComboAsync(
            int id,
            CancellationToken cancellationToken);

        Task<bool> TienePedidosAsync(
            int id,
            CancellationToken cancellationToken);

        Task EliminarAsync(
            int id,
            CancellationToken cancellationToken);

        Task GuardarAsync(
            CancellationToken cancellationToken);
    }
}
