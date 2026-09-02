using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb.Models
{
    public sealed record TmdbMovieSearchResponse(
     int Page,
     IReadOnlyCollection<TmdbMovieResponse> Results,
     int TotalPages,
     int TotalResults);
}
