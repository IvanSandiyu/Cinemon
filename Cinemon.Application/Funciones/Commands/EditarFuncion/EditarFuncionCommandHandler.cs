using Cinemon.Application.Abstractions;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.EditarFuncion
{
    public sealed class EditarFuncionCommandHandler: IRequestHandler<EditarFuncionCommand>
    {
        private readonly IFuncionRepository _funcionRepository;
        private readonly ISalaRepository _salaRepository;

        public EditarFuncionCommandHandler(IFuncionRepository funcionRepository, ISalaRepository salaRepository)
        {
            _funcionRepository = funcionRepository;
            _salaRepository = salaRepository;
        }

        public async Task Handle(EditarFuncionCommand request,CancellationToken cancellationToken)
        {
            var funcion = await _funcionRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (funcion is null)
                throw new NotFoundException(
                    "La función no existe.");

            if (funcion.EstadoFuncion != EstadoFuncion.Programada)
                throw new BusinessRuleException(
                    "Solo se pueden editar funciones programadas.");

            var pelicula = await _funcionRepository.ObtenerPeliculaAsync(
                request.PeliculaId,
                cancellationToken);

            if (pelicula is null)
                throw new NotFoundException("La película no existe.");

            var sala = await _salaRepository.ObtenerPorIdAsync(request.SalaId,cancellationToken);

            if (sala is null)
                throw new NotFoundException(
                    "La sala no existe.");

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
                    request.Id,
                    cancellationToken);

            if (existeSuperposicion)
                throw new ConflictException(
                    "La sala ya tiene una función programada en ese horario.");

            funcion.Editar(
                request.PeliculaId,
                request.SalaId,
                request.FechaHoraInicio,
                request.Idioma,
                request.Formato,
                request.Precio);

            await _funcionRepository.UpdateAsync(
                funcion,
                cancellationToken);
        }
    }
}
