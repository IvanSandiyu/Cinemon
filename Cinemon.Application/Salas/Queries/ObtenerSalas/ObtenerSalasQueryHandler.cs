using Cinemon.Application.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Salas.Queries.ObtenerSalas
{
    public sealed class ObtenerSalasQueryHandler: IRequestHandler<ObtenerSalasQuery,IReadOnlyCollection<SalaDto>>
    {
        private readonly ISalaRepository _salaRepository;

        public ObtenerSalasQueryHandler(ISalaRepository salaRepository)
        {
            _salaRepository = salaRepository;
        }

        public async Task<IReadOnlyCollection<SalaDto>> Handle(ObtenerSalasQuery request,CancellationToken cancellationToken)
        {
            var salas = await _salaRepository
                .ObtenerTodasAsync(cancellationToken);

            return salas
                .Select(sala => new SalaDto(
                    sala.Id,
                    sala.Numero,
                    sala.TipoSala.ToString(),
                    sala.Butacas.Count))
                .ToList();
        }
    }
}
