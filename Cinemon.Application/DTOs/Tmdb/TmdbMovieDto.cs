using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.DTOs.Tmdb
{
    public sealed record TmdbMovieDto(
    int Id,
    string Title,
    string? Overview,
    string? PosterPath,
    string? BackdropPath,
    DateTime? ReleaseDate,
    int? Runtime,
    string? Certificacion);
}
