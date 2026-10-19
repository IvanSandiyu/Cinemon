using Cinemon.Application.Abstractions;
using Cinemon.Application.Common;
using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Queries.ObtenerFunciones
{
    public sealed class ObtenerFuncionesQueryHandler : IRequestHandler<ObtenerFuncionesQuery, IReadOnlyCollection<FuncionDto>>
    {
        private readonly IFuncionRepository _funcionRepository;
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly TimeProvider _timeProvider;

        public ObtenerFuncionesQueryHandler(
            IFuncionRepository funcionRepository,
            IPeliculaRepository peliculaRepository,
            TimeProvider timeProvider)
        {
            _funcionRepository = funcionRepository;
            _peliculaRepository = peliculaRepository;
            _timeProvider = timeProvider;
        }

        public async Task<IReadOnlyCollection<FuncionDto>> Handle(ObtenerFuncionesQuery request, CancellationToken cancellationToken)
        {
            var funciones = await _funcionRepository.ObtenerFuncionesAsync(cancellationToken);
            var peliculas = await _peliculaRepository.ObtenerTodasAsync(cancellationToken);

            var duraciones = peliculas.ToDictionary(p => p.Id, p => p.Duracion);
            var ahora = _timeProvider.Ahora();

            return funciones.Select(funcion => new FuncionDto(
                funcion.Id,
                funcion.PeliculaId,
                funcion.SalaId,
                funcion.FechaHoraInicio,
                funcion.Idioma,
                funcion.Formato,
                funcion.Precio,
                CalcularEstado(funcion, duraciones, ahora))).ToList();
        }

        private static EstadoFuncion CalcularEstado(
            Funcion funcion,
            IReadOnlyDictionary<int, int> duraciones,
            DateTime ahora)
        {
            if (funcion.EstadoFuncion != EstadoFuncion.Programada)
                return funcion.EstadoFuncion;

            if (!duraciones.TryGetValue(funcion.PeliculaId, out var duracion))
                return funcion.EstadoFuncion;

            var fechaHoraFin = funcion.FechaHoraInicio.AddMinutes(duracion);

            return fechaHoraFin <= ahora
                ? EstadoFuncion.Finalizada
                : EstadoFuncion.Programada;
        }
    }
}
