using Cinemon.Application.Abstractions;
using Cinemon.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Commands.VincularTmdb
{
    public sealed class VincularTmdbCommandHandler: IRequestHandler<VincularTmdbCommand>
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly ITmdbService _tmdbService;

        public VincularTmdbCommandHandler(IPeliculaRepository peliculaRepository,ITmdbService tmdbService)
        {
            _peliculaRepository = peliculaRepository;
            _tmdbService = tmdbService;
        }

        public async Task Handle(VincularTmdbCommand request,CancellationToken cancellationToken)
        {
            var pelicula = await _peliculaRepository.ObtenerPorIdAsync(
                request.PeliculaId,
                cancellationToken);

            if (pelicula is null)
                throw new KeyNotFoundException(
                    "La película no existe.");

            var tmdbMovie = await _tmdbService.ObtenerPeliculaAsync(
                request.TmdbId,
                cancellationToken);

            if (tmdbMovie is null)
                throw new KeyNotFoundException(
                    "La película no existe en TMDB.");

            pelicula.AsignarTmdb(tmdbMovie.Id,tmdbMovie.PosterPath,tmdbMovie.BackdropPath);

            await _peliculaRepository.UpdateAsync(
                pelicula,
                cancellationToken);
        }
    }
}
