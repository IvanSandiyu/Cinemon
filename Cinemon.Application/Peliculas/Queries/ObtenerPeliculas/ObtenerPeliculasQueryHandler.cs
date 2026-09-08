using Cinemon.Application.Abstractions;
using Cinemon.Application.Common;
using Cinemon.Domain.Entidades.Peliculas;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Queries.ObtenerPeliculas
{
    public sealed class ObtenerPeliculasQueryHandler: IRequestHandler<ObtenerPeliculasQuery,IReadOnlyCollection<PeliculaDto>>
    {
        private readonly IPeliculaRepository _peliculaRepository;

        public ObtenerPeliculasQueryHandler(IPeliculaRepository peliculaRepository)
        {
            _peliculaRepository = peliculaRepository;
        }

        public async Task<IReadOnlyCollection<PeliculaDto>> Handle(ObtenerPeliculasQuery request,CancellationToken cancellationToken)
        {
            var peliculas = await _peliculaRepository
                .ObtenerTodasAsync(cancellationToken);

            return peliculas
                .Select(pelicula => new PeliculaDto(
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
                TmdbImageUrlHelper.BuildBackdropUrl(pelicula.TmdbBackdropPath))).ToList();
        }
    }
}
