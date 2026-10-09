using System.Text.Json.Serialization;

namespace Cinemon.Infrastructure.ExternalServices.Tmdb.Models
{
    public sealed record TmdbImageResponse(
     [property: JsonPropertyName("file_path")]string FilePath,
     [property: JsonPropertyName("width")]int Width,
     [property: JsonPropertyName("height")]int Height,
     [property: JsonPropertyName("iso_639_1")]string? Language,
     [property: JsonPropertyName("vote_average")]double VoteAverage,
     [property: JsonPropertyName("vote_count")]int VoteCount);
}