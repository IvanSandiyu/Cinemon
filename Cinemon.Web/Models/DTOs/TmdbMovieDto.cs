namespace Cinemon.Web.Models.DTOs
{
    public sealed record TmdbMovieDto(
     int Id,
     string Title,
     string Overview,
     string? PosterPath,
     string? BackdropPath,
     DateTime? ReleaseDate,
     int? Runtime,
     string? Certificacion,
     IReadOnlyCollection<string> Generos);
}
