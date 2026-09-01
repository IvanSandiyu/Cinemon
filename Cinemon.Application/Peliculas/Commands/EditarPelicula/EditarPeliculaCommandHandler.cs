using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Peliculas;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Commands.EditarPelicula
{
    public class EditarPeliculaCommandHandler : IRequestHandler<EditarPeliculaCommand, int>
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly IGeneroRepository _generoRepository;

        public EditarPeliculaCommandHandler(IPeliculaRepository peliculaRepository, IGeneroRepository generoRepository)
        {
            _peliculaRepository = peliculaRepository;
            _generoRepository = generoRepository;
        }
        public async Task<int> Handle(EditarPeliculaCommand request, CancellationToken cancellationToken)
        {
            var generosExistentes =await _generoRepository.ExistAllAsync(request.GeneroIds,cancellationToken);

            if (!generosExistentes) {
                throw new InvalidOperationException(
                    "Uno o más géneros no existen.");
            }

            var pelicula = await _peliculaRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (pelicula is null) {
                throw new InvalidOperationException(
                    "La película no existe.");
            }

            pelicula.Actualizar(
                request.Titulo,
                request.Sinopsis,
                request.Duracion,
                request.FechaEstreno,
                request.ClasificacionEdad,
                request.PosterUrl,
                request.TrailerUrl);

            await _peliculaRepository.UpdateAsync(pelicula,request.GeneroIds,cancellationToken);

            return pelicula.Id;
        }
    }
}
