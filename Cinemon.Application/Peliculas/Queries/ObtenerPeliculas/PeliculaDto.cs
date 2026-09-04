using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Queries.ObtenerPeliculas
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
