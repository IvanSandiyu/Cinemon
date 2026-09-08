using Cinemon.Application.DTOs.Tmdb;
using Cinemon.Application.Interfaces;
using MediatR;

namespace Cinemon.Application.Tmdb.Queries.BuscarPeliculasTmdb
{
    public sealed class ObtenerPeliculaTmdbQueryHandler: IRequestHandler<ObtenerPeliculaTmdbQuery, TmdbMovieDto?>
    {
        private readonly ITmdbService _tmdbService;

        public ObtenerPeliculaTmdbQueryHandler(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        public async Task<TmdbMovieDto?> Handle(ObtenerPeliculaTmdbQuery request, CancellationToken cancellationToken)
        {
            return await _tmdbService.ObtenerPeliculaAsync(
                request.TmdbId,
                cancellationToken);
        }
    }
}