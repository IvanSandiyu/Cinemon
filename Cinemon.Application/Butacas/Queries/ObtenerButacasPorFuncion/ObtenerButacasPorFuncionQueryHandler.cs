using Cinemon.Application.Abstractions;
using Cinemon.Application.Features.Butacas.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Butacas.Queries.ObtenerButacasPorFuncion
{
    public sealed class ObtenerButacasPorFuncionQueryHandler: IRequestHandler<ObtenerButacasPorFuncionQuery,IReadOnlyCollection<ButacaFuncionDto>>
    {
        private readonly IButacaRepository _butacaRepository;

        public ObtenerButacasPorFuncionQueryHandler(IButacaRepository butacaRepository)
        {
            _butacaRepository = butacaRepository;
        }

        public async Task<IReadOnlyCollection<ButacaFuncionDto>> Handle(ObtenerButacasPorFuncionQuery request,CancellationToken cancellationToken)
        {
            var butacas = await _butacaRepository
                .ObtenerPorFuncionAsync(
                    request.FuncionId,
                    cancellationToken);

            var ocupadas = await _butacaRepository
                .ObtenerIdsOcupadosPorFuncionAsync(
                    request.FuncionId,
                    cancellationToken);

            var ocupadasSet = ocupadas.ToHashSet();

            return butacas
                .Select(butaca => new ButacaFuncionDto(
                    butaca.Id,
                    butaca.Fila,
                    butaca.Numero,
                    ocupadasSet.Contains(butaca.Id)))
                .ToList();
        }
    }
}
