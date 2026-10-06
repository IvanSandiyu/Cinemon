using Cinemon.Application.Abstractions;
using Cinemon.Application.DTOs.Promocion;
using Cinemon.Domain.Entidades.Promociones;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Application.Promociones.Queries
{
    public sealed class ObtenerPromocionesQueryHandler
    : IRequestHandler<ObtenerPromocionesQuery,IReadOnlyCollection<PromocionDto>>
    {
        private readonly IPromocionRepository _promocionRepository;

        public ObtenerPromocionesQueryHandler(IPromocionRepository promocionRepository)
        {
            _promocionRepository = promocionRepository;
        }

        public async Task<IReadOnlyCollection<PromocionDto>> Handle(
            ObtenerPromocionesQuery request,
            CancellationToken cancellationToken)
        {
            var promociones = await _promocionRepository.ObtenerTodasAsync(
                cancellationToken);

            return promociones
                .Select(Mapear)
                .ToList();
        }

        public static PromocionDto Mapear(Promocion promocion)
        {
            return new PromocionDto(
                promocion.Id,
                promocion.Nombre,
                promocion.Descripcion,
                (int)promocion.Tipo,
                promocion.CantidadPagadas,
                promocion.CantidadGratis,
                promocion.PorcentajeDescuento,
                promocion.FechaDesde,
                promocion.FechaHasta,
                promocion.Activa,
                promocion.DiasSemana
                    .Select(x => (int)x.Dia)
                    .OrderBy(x => x)
                    .ToList());
        }
    }
}
