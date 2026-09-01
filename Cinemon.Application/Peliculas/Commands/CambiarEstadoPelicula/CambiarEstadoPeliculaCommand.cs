using MediatR;

namespace Cinemon.Application.Peliculas.Commands.CambiarEstadoPelicula
{
    public sealed record CambiarEstadoPeliculaCommand(
        int Id,
        bool Activa) : IRequest<bool>;
}