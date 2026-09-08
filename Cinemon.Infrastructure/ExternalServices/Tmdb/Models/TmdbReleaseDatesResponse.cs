using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb.Models
{
    public sealed record TmdbReleaseDatesResponse(
        [property: JsonPropertyName("results")]
        IReadOnlyCollection<TmdbCountryRelease>? Results);

    public sealed record TmdbCountryRelease(
        [property: JsonPropertyName("iso_3166_1")]
        string? Iso31661,
        [property: JsonPropertyName("release_dates")]
        IReadOnlyCollection<TmdbReleaseDateEntry>? ReleaseDates);

    public sealed record TmdbReleaseDateEntry(
        [property: JsonPropertyName("certification")]
        string? Certification);
}
