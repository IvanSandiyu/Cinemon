using Cinemon.Application.DTOs.Tmdb;
using Cinemon.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Tmdb.Queries.BuscarPeliculasTmdb
{
    public sealed class BuscarPeliculasTmdbQueryHandler: IRequestHandler<BuscarPeliculasTmdbQuery,IReadOnlyCollection<TmdbMovieDto>>
    {
        private readonly ITmdbService _tmdbService;

        public BuscarPeliculasTmdbQueryHandler(ITmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        public async Task<IReadOnlyCollection<TmdbMovieDto>> Handle(BuscarPeliculasTmdbQuery request,CancellationToken cancellationToken)
        {
            return await _tmdbService.BuscarPeliculasAsync(
                request.Query,
                cancellationToken);
        }
    }
}
