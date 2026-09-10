namespace Cinemon.Web.Models.DTOs.Requests
{
    public sealed record EditarPeliculaRequest(
    string Titulo,
    string Sinopsis,
    int Duracion,
    DateTime FechaEstreno,
    int ClasificacionEdad,
    string PosterUrl,
    string TrailerUrl,
    IReadOnlyCollection<int> GeneroIds);
}