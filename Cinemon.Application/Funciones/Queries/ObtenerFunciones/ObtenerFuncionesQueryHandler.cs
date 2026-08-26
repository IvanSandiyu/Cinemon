using Cinemon.Application.Abstractions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Queries.ObtenerFunciones
{
    public sealed class ObtenerFuncionesQueryHandler : IRequestHandler<ObtenerFuncionesQuery, IReadOnlyCollection<FuncionDto>>
    {
        private readonly IFuncionRepository _funcionRepository;
       public ObtenerFuncionesQueryHandler(IFuncionRepository funcionRepository)
       {
            _funcionRepository = funcionRepository;
       }
        public async Task<IReadOnlyCollection<FuncionDto>> Handle(ObtenerFuncionesQuery request, CancellationToken cancellationToken)
        {
            var funciones = await _funcionRepository.ObtenerFuncionesAsync(cancellationToken);

            return funciones.Select(funcion => new FuncionDto(
                funcion.PeliculaId,
                funcion.SalaId,
                funcion.FechaHoraInicio,
                funcion.Idioma,
                funcion.Formato,
                funcion.Precio,
                new List<string>())).ToList();
        }
    }
}
