using Cinemon.Domain.Enums;

namespace Cinemon.Api.Endpoints.Peliculas
{
    public sealed record CrearPeliculaRequest(
     string Titulo,
     string Sinopsis,
     int Duracion,
     DateTime FechaEstreno,
     ClasificacionEdad ClasificacionEdad,
     string PosterUrl,
     string TrailerUrl,
     List<int> GeneroIds);
}
