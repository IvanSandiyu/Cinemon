namespace Cinemon.Web.Models.DTOs
{
    public sealed record PeliculaDto(
    int Id,
    string Titulo,
    string Sinopsis,
    int Duracion,
    DateTime FechaEstreno,
    string ClasificacionEdad,
    string PosterUrl,
    string TrailerUrl,
    bool Activa,
    IReadOnlyCollection<string> Generos,
    string? TmdbPosterUrl,
    string? TmdbBackdropUrl);
}
