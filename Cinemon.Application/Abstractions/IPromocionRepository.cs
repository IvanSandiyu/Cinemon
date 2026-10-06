using Cinemon.Domain.Entidades.Promociones;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IPromocionRepository
    {
        Task<IReadOnlyCollection<Promocion>> ObtenerTodasAsync(
            CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Promocion>> ObtenerActivasParaFechaAsync(
            DateTime fechaHoraFuncion,
            CancellationToken cancellationToken);

        Task<Promocion?> ObtenerPorIdAsync(
            int id,
            CancellationToken cancellationToken);

        Task AddAsync(
            Promocion promocion,
            CancellationToken cancellationToken);

        void Update(Promocion promocion);

        Task EliminarAsync(
            int id,
            CancellationToken cancellationToken);

        Task GuardarAsync(
            CancellationToken cancellationToken);
    }
}
