using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.CrearFuncion
{
    public sealed class CrearFuncionCommandHandler: IRequestHandler<CrearFuncionCommand, int>
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly ISalaRepository _salaRepository;
        private readonly IFuncionRepository _funcionRepository;

        public CrearFuncionCommandHandler(IPeliculaRepository peliculaRepository,ISalaRepository salaRepository,
            IFuncionRepository funcionRepository)
        {
            _peliculaRepository = peliculaRepository;
            _salaRepository = salaRepository;
            _funcionRepository = funcionRepository;
        }

        public async Task<int> Handle(CrearFuncionCommand request,CancellationToken cancellationToken)
        {
            var pelicula = await _peliculaRepository.ObtenerPorIdAsync(
                request.PeliculaId,
                cancellationToken);

            if (pelicula is null) {
                throw new NotFoundException(
                    "La película no existe.");
            }else {
                pelicula.Activar();
            }

            if (!pelicula.Activa) {
                throw new BusinessRuleException(
                    "La película no está activa.");
            }

            var sala = await _salaRepository.ObtenerPorIdAsync(
                request.SalaId,
                cancellationToken);

            if (sala is null) {
                throw new NotFoundException(
                    "La sala no existe.");
            }

            if (sala.TipoSala == TipoSala.Imax &&
                request.Formato == Formato.TresD) {
                throw new BusinessRuleException(
                    "Una sala IMAX no puede tener funciones 3D.");
            }

            var fechaHoraFin = request.FechaHoraInicio
                .AddMinutes(pelicula.Duracion);

            var existeSuperposicion =
                await _funcionRepository.ExisteSuperposicionAsync(
                    request.SalaId,
                    request.FechaHoraInicio,
                    fechaHoraFin,
                    cancellationToken);

            if (existeSuperposicion) {
                throw new ConflictException(
                    "La sala ya tiene una función programada en ese horario.");
            }

            var funcion = new Funcion(
                request.PeliculaId,
                request.SalaId,
                request.FechaHoraInicio,
                request.Idioma,
                request.Formato,
                request.Precio);

            await _funcionRepository.AddAsync(
                funcion,
                cancellationToken);

            return funcion.Id;
        }
    }
}
