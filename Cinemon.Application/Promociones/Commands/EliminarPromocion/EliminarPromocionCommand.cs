using MediatR;

namespace Cinemon.Application.Promociones.Commands.EliminarPromocion
{
    public sealed record EliminarPromocionCommand(int Id) : IRequest<Unit>;
}
