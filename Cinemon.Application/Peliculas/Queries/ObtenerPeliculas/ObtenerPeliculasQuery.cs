using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Queries.ObtenerPeliculas
{

    public sealed record ObtenerPeliculasQuery: IRequest<IReadOnlyCollection<PeliculaDto>>;

}
