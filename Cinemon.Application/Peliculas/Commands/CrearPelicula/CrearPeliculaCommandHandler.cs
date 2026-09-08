using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Peliculas;
using Cinemon.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Commands.CrearPelicula
{
    //El handler verifica que cada entidad exista
    public class CrearPeliculaCommandHandler:IRequestHandler<CrearPeliculaCommand, int>
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly IGeneroRepository _generoRepository;

        public CrearPeliculaCommandHandler(IPeliculaRepository peliculaRepository,IGeneroRepository generoRepository)
        {
            _peliculaRepository = peliculaRepository;
            _generoRepository = generoRepository;
        }

        public async Task<int> Handle(CrearPeliculaCommand request,CancellationToken cancellationToken)
        {
            var generosExistentes =
                await _generoRepository.ExistAllAsync(
                    request.GeneroIds,
                    cancellationToken);

            if (!generosExistentes) {
                throw new NotFoundException(
                    "Uno o más géneros no existen.");
            }

            var pelicula = new Pelicula(
                request.Titulo,
                request.Sinopsis,
                request.Duracion,
                request.FechaEstreno,
                request.ClasificacionEdad,
                request.PosterUrl,
                request.TrailerUrl);

            await _peliculaRepository.AddAsync(
                pelicula,
                request.GeneroIds,
                cancellationToken);

            return pelicula.Id;
        }
    }
}
