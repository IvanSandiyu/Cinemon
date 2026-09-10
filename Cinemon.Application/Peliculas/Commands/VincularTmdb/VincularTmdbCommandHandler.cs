using Cinemon.Application.Abstractions;
using Cinemon.Application.Interfaces;
using Cinemon.Domain.Exceptions;
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
        private readonly IGeneroRepository _generoRepository;

        public VincularTmdbCommandHandler(IPeliculaRepository peliculaRepository,ITmdbService tmdbService, IGeneroRepository generoRepository)
        {
            _peliculaRepository = peliculaRepository;
            _tmdbService = tmdbService;
            _generoRepository = generoRepository;
        }

        public async Task Handle(VincularTmdbCommand request, CancellationToken cancellationToken)
        {
            var pelicula = await _peliculaRepository.ObtenerPorIdAsync(
                request.PeliculaId,
                cancellationToken);

            if (pelicula is null)
                throw new NotFoundException(
                    "La película no existe.");

            var tmdbMovie = await _tmdbService.ObtenerPeliculaAsync(
                request.TmdbId,
                cancellationToken);

            if (tmdbMovie is null)
                throw new NotFoundException(
                    "La película no existe en TMDB.");

            var generos = await _generoRepository.ObtenerOCrearPorNombresAsync(
                tmdbMovie.Generos,
                cancellationToken);

            var generoIds = generos
                .Select(x => x.Id)
                .ToList();

            pelicula.AsignarTmdb(
                tmdbMovie.Id,
                tmdbMovie.PosterPath,
                tmdbMovie.BackdropPath);

            pelicula.ActualizarDatosDesdeTmdb(
                tmdbMovie.Overview,
                tmdbMovie.Runtime);

            await _peliculaRepository.UpdateAsync(
                pelicula,
                generoIds,
                cancellationToken);
        }
    }
}
