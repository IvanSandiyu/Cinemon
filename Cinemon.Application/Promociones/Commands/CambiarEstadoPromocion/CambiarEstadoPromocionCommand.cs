using MediatR;

namespace Cinemon.Application.Promociones.Commands.CambiarEstadoPromocion
{
    public sealed record CambiarEstadoPromocionCommand(
     int Id,
     bool Activa) : IRequest<Unit>;
}
