using Cinemon.Application.DTOs.Candy;
using MediatR;

namespace Cinemon.Application.Candy.Pedidos.Queries.ObtenerMisPedidosCandy
{
    public sealed record ObtenerMisPedidosCandyQuery
        : IRequest<IReadOnlyCollection<PedidoCandyDto>>;
}
