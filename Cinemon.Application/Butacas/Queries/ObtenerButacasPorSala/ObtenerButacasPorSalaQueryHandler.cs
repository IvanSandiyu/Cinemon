using Cinemon.Application.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Butacas.Queries.ObtenerButacasPorSala
{
    public sealed class ObtenerButacasPorSalaQueryHandler: IRequestHandler<ObtenerButacasPorSalaQuery,IReadOnlyCollection<ButacaDto>>
    {
        private readonly IButacaRepository _butacaRepository;

        public ObtenerButacasPorSalaQueryHandler(IButacaRepository butacaRepository)
        {
            _butacaRepository = butacaRepository;
        }

        public async Task<IReadOnlyCollection<ButacaDto>> Handle(
            ObtenerButacasPorSalaQuery request,
            CancellationToken cancellationToken)
        {
            var butacas = await _butacaRepository
                .ObtenerPorSalaAsync(
                    request.SalaId,
                    cancellationToken);

            return butacas
                .Select(butaca => new ButacaDto(
                    butaca.Id,
                    butaca.Fila,
                    butaca.Numero))
                .ToList();
        }
    }
}
