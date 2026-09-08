using Cinemon.Application.Abstractions;
using Cinemon.Application.Common;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Application.Peliculas.Queries.ObtenerPeliculas
{
    public sealed class ObtenerPeliculasPorIdQueryHandler: IRequestHandler<ObtenerPeliculasPorIdQuery, PeliculaDto?>
    {
        private readonly IPeliculaRepository _peliculaRepository;

        public ObtenerPeliculasPorIdQueryHandler(IPeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        public async Task<PeliculaDto?> Handle(ObtenerPeliculasPorIdQuery request,CancellationToken cancellationToken)
        {
            var pelicula = await _peliculaRepository
                .ObtenerPorIdAsync(request.Id, cancellationToken);

            if (pelicula is null)
                return null;

            return new PeliculaDto(
                pelicula.Id,
                pelicula.Titulo,
                pelicula.Sinopsis,
                pelicula.Duracion,
                pelicula.FechaEstreno,
                ClasificacionEdadHelper.ParaMostrar(pelicula.ClasificacionEdad),
                pelicula.PosterUrl,
                pelicula.TrailerUrl,
                pelicula.Activa,
                pelicula.Generos.Select(g => g.Genero.Nombre).ToList(), 
                TmdbImageUrlHelper.BuildPosterUrl(pelicula.TmdbPosterPath),
                TmdbImageUrlHelper.BuildBackdropUrl(pelicula.TmdbBackdropPath));
        }
    }
}
