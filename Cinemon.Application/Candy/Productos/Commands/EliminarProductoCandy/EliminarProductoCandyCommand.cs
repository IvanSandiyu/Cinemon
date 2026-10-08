using MediatR;

namespace Cinemon.Application.Candy.Productos.Commands.EliminarProductoCandy
{
    public sealed record EliminarProductoCandyCommand(
        int Id) : IRequest<Unit>;
}
