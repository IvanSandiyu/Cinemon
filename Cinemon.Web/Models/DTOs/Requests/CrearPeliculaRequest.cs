namespace Cinemon.Web.Models.DTOs.Requests
{
    public sealed record CrearPeliculaRequest(
    string Titulo,
    string Sinopsis,
    int Duracion,
    DateTime FechaEstreno,
    string ClasificacionEdad,
    string PosterUrl,
    string TrailerUrl,
    IReadOnlyCollection<int> GeneroIds);
}
