using Cinemon.Application.Reservas.Queries.ObtenerReservas;
using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Entidades.Reservas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Abstractions
{
    public interface IReservaRepository
    {
        Task<bool> ButacasDisponiblesAsync(int funcionId, IReadOnlyCollection<int> butacasIds, CancellationToken cancellationToken);

        Task AddAsync(Reserva reserva, IReadOnlyCollection<int> butacasIds, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Reserva>> ObtenerTodasAsync(CancellationToken cancellationToken);

        Task<IReadOnlyCollection<Reserva>> ObtenerPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken);

        Task<Reserva?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken);

        Task<bool> CancelarAsync(int id, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<int>> ObtenerButacaIdsAsync(int reservaId, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<ReservaDto>> ObtenerReservasAsync(int? usuarioId,CancellationToken cancellationToken);

        Task<ReservaDto?> ObtenerDetalleAsync(int id,CancellationToken cancellationToken);
    }
}
