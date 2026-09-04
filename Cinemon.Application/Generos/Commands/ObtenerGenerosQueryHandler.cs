using Cinemon.Application.Abstractions;
using Cinemon.Application.DTOs.Genero;
using Cinemon.Application.Generos.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Generos.Commands
{
    public sealed class ObtenerGenerosQueryHandler: IRequestHandler<ObtenerGenerosQuery, IReadOnlyCollection<GeneroDto>>
    {
        private readonly IGeneroRepository _generoRepository;

        public ObtenerGenerosQueryHandler(IGeneroRepository generoRepository)
        {
            _generoRepository = generoRepository;
        }

        public async Task<IReadOnlyCollection<GeneroDto>> Handle(ObtenerGenerosQuery request,CancellationToken cancellationToken)
        {
            var generos = await _generoRepository.ObtenerTodosAsync(cancellationToken);

            return generos.Select(x => new GeneroDto(x.Id, x.Nombre)).ToList();
        }
    }
}
