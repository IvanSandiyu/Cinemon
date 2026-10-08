using MediatR;

namespace Cinemon.Application.Candy.Productos.Commands.CambiarEstadoProductoCandy
{
    public sealed record CambiarEstadoProductoCandyCommand(
        int Id,
        bool Activo) : IRequest<Unit>;
}
