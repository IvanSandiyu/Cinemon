using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb.Models
{
    public sealed record TmdbImagesResponse(
     [property: JsonPropertyName("posters")]IReadOnlyCollection<TmdbImageResponse>? Posters);
}