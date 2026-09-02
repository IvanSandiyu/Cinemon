using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb.Models
{
    public sealed record TmdbMovieResponse(
     int Id,
     string Title,
     string? Overview,
     string? PosterPath,
     string? BackdropPath,
     string? ReleaseDate);
}
