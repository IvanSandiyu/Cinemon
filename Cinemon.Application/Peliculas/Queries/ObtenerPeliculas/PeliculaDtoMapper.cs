using Cinemon.Application.Common;
using Cinemon.Domain.Entidades.Peliculas;

namespace Cinemon.Application.Peliculas.Queries.ObtenerPeliculas
{
    internal static class PeliculaDtoMapper
    {
        public static PeliculaDto ToDto(Pelicula pelicula) =>
            new(
                pelicula.Id,
                pelicula.Titulo,
                pelicula.Sinopsis,
                pelicula.Duracion,
                pelicula.FechaEstreno,
                ClasificacionEdadHelper.ParaMostrar(pelicula.ClasificacionEdad),
                pelicula.PosterUrl,
                TmdbImageUrlHelper.ObtenerTrailerUrl(
                    pelicula.TrailerUrl,
                    pelicula.TmdbTrailerKey),
                pelicula.Activa,
                pelicula.Generos.Select(g => g.Genero.Nombre).ToList(),
                TmdbImageUrlHelper.BuildPosterUrl(pelicula.TmdbPosterPath),
                TmdbImageUrlHelper.BuildBackdropUrl(pelicula.TmdbBackdropPath),
                pelicula.Posters
                    .Select(p => TmdbImageUrlHelper.BuildPosterUrl(p.Ruta))
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .Cast<string>()
                    .ToList());
    }
}
