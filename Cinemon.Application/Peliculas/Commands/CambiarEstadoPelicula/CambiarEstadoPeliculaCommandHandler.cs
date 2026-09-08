using Cinemon.Application.Abstractions;
using MediatR;

namespace Cinemon.Application.Peliculas.Commands.CambiarEstadoPelicula
{
    public sealed class CambiarEstadoPeliculaCommandHandler
        : IRequestHandler<CambiarEstadoPeliculaCommand, bool>
    {
        private readonly IPeliculaRepository _peliculaRepository;

        public CambiarEstadoPeliculaCommandHandler(IPeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        public async Task<bool> Handle(CambiarEstadoPeliculaCommand request,CancellationToken cancellationToken)
        {
            return await _peliculaRepository.CambiarEstadoActivaAsync(
                request.Id,
                request.Activa,
                cancellationToken);
        }
    }
}