using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace Cinemon.Infrastructure.ExternalServices.Tmdb.Models
{
    public sealed record TmdbMovieResponse(
     [property: JsonPropertyName("id")]int Id,

     [property: JsonPropertyName("title")]string Title,

     [property: JsonPropertyName("overview")]string Overview,

     [property: JsonPropertyName("poster_path")]string? PosterPath,

     [property: JsonPropertyName("backdrop_path")]string? BackdropPath,

     [property: JsonPropertyName("release_date")]string? ReleaseDate,

     [property: JsonPropertyName("runtime")]int? Runtime,
    [property: JsonPropertyName("genres")]IReadOnlyCollection<TmdbGeneroResponse>? Generos,
     [property: JsonPropertyName("release_dates")]TmdbReleaseDatesResponse? ReleaseDates);
}
