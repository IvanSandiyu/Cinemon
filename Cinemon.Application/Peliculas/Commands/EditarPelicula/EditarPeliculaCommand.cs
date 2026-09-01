using Cinemon.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Commands.EditarPelicula
{
    public record EditarPeliculaCommand(
        int Id,
        string Titulo,
        string Sinopsis,
        int Duracion,
        DateTime FechaEstreno,
        ClasificacionEdad ClasificacionEdad,
        string PosterUrl,
        string TrailerUrl,
        List<int> GeneroIds) : IRequest<int>;
}
