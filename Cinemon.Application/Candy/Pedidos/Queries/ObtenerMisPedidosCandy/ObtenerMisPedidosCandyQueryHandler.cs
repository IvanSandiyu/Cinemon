using Cinemon.Application.Abstractions;
using Cinemon.Application.DTOs.Candy;
using Cinemon.Application.Interfaces;
using MediatR;
using System.Linq;

namespace Cinemon.Application.Candy.Pedidos.Queries.ObtenerMisPedidosCandy
{
    public sealed class ObtenerMisPedidosCandyQueryHandler
        : IRequestHandler<ObtenerMisPedidosCandyQuery, IReadOnlyCollection<PedidoCandyDto>>
    {
        private readonly IPedidoCandyRepository _pedidoCandyRepository;
        private readonly ICurrentUserService _currentUserService;

        public ObtenerMisPedidosCandyQueryHandler(
            IPedidoCandyRepository pedidoCandyRepository,
            ICurrentUserService currentUserService)
        {
            _pedidoCandyRepository = pedidoCandyRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IReadOnlyCollection<PedidoCandyDto>> Handle(
            ObtenerMisPedidosCandyQuery request,
            CancellationToken cancellationToken)
        {
            var pedidos = await _pedidoCandyRepository.ObtenerPorUsuarioAsync(
                _currentUserService.UserId,
                cancellationToken);

            return pedidos.Select(pedido => new PedidoCandyDto(
                pedido.Id,
                pedido.FechaPedido,
                pedido.Total,
                pedido.Items
                    .Select(item => new PedidoCandyItemDto(
                        item.ProductoId,
                        item.NombreProducto,
                        item.PrecioUnitario,
                        item.Cantidad,
                        item.Subtotal))
                    .ToList()))
                .ToList();
        }
    }
}
