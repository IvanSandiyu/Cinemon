using Cinemon.Application.DTOs.Tmdb;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Tmdb.Queries.BuscarPeliculasTmdb
{
    public sealed record BuscarPeliculasTmdbQuery(string Query) : IRequest<IReadOnlyCollection<TmdbMovieDto>>;
    public sealed record ObtenerPeliculaTmdbQuery(int TmdbId) : IRequest<TmdbMovieDto?>;
}
