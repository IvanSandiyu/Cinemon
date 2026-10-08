using Cinemon.Application.DTOs.Candy;
using MediatR;
using System.Collections.Generic;

namespace Cinemon.Application.Candy.Pedidos.Commands.CrearPedidoCandy
{
    public sealed record CrearPedidoCandyCommand(
        IReadOnlyCollection<CrearPedidoCandyItemRequest> Items) : IRequest<int>;
}
