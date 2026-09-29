using Cinemon.Application.Abstractions;
using Cinemon.Application.DTOs.Precio;
using Cinemon.Application.Precios.Queries;
using Cinemon.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Precios.Queries
{
    public sealed class ObtenerPreciosQueryHandler: IRequestHandler<ObtenerPreciosQuery,IReadOnlyCollection<PrecioDto>>
    {
        private readonly IPrecioRepository _precioRepository;

        public ObtenerPreciosQueryHandler(IPrecioRepository precioRepository)
        {
            _precioRepository = precioRepository;
        }

        public async Task<IReadOnlyCollection<PrecioDto>> Handle(ObtenerPreciosQuery request,
            CancellationToken cancellationToken)
        {
            var precios = await _precioRepository.ObtenerTodosAsync(
                cancellationToken);

            return precios
                .Select(x => new PrecioDto(
                    x.Id,
                    (int)x.Formato,
                    (int)x.TipoSala,
                    ObtenerNombre(x.Formato,x.TipoSala),
                    x.Valor))
                .ToList();
        }

        public static string ObtenerNombre(Formato formato,TipoSala tipoSala)
        {
            if (tipoSala == TipoSala.Imax) {
                return formato switch {
                    Formato.TresD => "IMAX 3D",
                    Formato.CuatroD => "IMAX 4D",
                    _ => "IMAX"
                };
            }

            return formato switch {
                Formato.TresD => "3D",
                Formato.CuatroD => "4D",
                _ => "General (2D)"
            };
        }
    }
}
