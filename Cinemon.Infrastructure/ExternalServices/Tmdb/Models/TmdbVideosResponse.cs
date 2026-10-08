using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb.Models
{
    public sealed record TmdbVideosResponse(
        [property: JsonPropertyName("results")]
        IReadOnlyCollection<TmdbVideoResponse>? Results);

    public sealed record TmdbVideoResponse(
        [property: JsonPropertyName("key")]
        string Key,
        [property: JsonPropertyName("site")]
        string? Site,
        [property: JsonPropertyName("type")]
        string? Type,
        [property: JsonPropertyName("official")]
        bool Official,
        [property: JsonPropertyName("iso_639_1")]
        string? Language,
        [property: JsonPropertyName("iso_3166_1")]
        string? Country,
        [property: JsonPropertyName("published_at")]
        string? PublishedAt);
}