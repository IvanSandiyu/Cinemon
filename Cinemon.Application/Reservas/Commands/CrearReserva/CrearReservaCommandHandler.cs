using Cinemon.Application.Abstractions;
using Cinemon.Application.Interfaces;
using Cinemon.Domain.Entidades.Butacas;
using Cinemon.Domain.Entidades.Reservas;
using Cinemon.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Reservas.Commands.CrearReserva
{
    public sealed class CrearReservaCommandHandler: IRequestHandler<CrearReservaCommand, int>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IFuncionRepository _funcionRepository;
        private readonly IButacaRepository _butacaRepository;
        private readonly IReservaRepository _reservaRepository;
        private readonly ICurrentUserService _currentUserService;

        public CrearReservaCommandHandler(
            IUsuarioRepository usuarioRepository,
            IFuncionRepository funcionRepository,
            IButacaRepository butacaRepository,
            IReservaRepository reservaRepository,
            ICurrentUserService currentUserService)
        {
            _usuarioRepository = usuarioRepository;
            _funcionRepository = funcionRepository;
            _butacaRepository = butacaRepository;
            _reservaRepository = reservaRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(
            CrearReservaCommand request,
            CancellationToken cancellationToken)
        {
            var usuario = await _usuarioRepository.ObtenerPorIdAsync(
                _currentUserService.UserId,
                cancellationToken);

            if (usuario is null) {
                throw new InvalidOperationException(
                    "El usuario no existe.");
            }

            if (!usuario.Activo) {
                throw new InvalidOperationException(
                    "El usuario no está activo.");
            }

            var realizadaPor = await _usuarioRepository.ObtenerPorIdAsync(
               _currentUserService.UserId,
                cancellationToken);

            if (realizadaPor is null) {
                throw new InvalidOperationException(
                    "El usuario que realiza la reserva no existe.");
            }

            if (!realizadaPor.Activo) {
                throw new InvalidOperationException(
                    "El usuario que realiza la reserva no está activo.");
            }

            var funcion = await _funcionRepository.ObtenerPorIdAsync(
                request.FuncionId,
                cancellationToken);

            if (funcion is null) {
                throw new InvalidOperationException(
                    "La función no existe.");
            }

            if (funcion.EstadoFuncion != EstadoFuncion.Programada) {
                throw new InvalidOperationException(
                    "La función no está disponible para realizar reservas.");
            }

            var butacas = await _butacaRepository.ObtenerPorIdsAsync(
                request.ButacasIds,
                cancellationToken);

            if (butacas.Count != request.ButacasIds.Count) {
                throw new InvalidOperationException(
                    "Una o más butacas no existen.");
            }

            if (butacas.Any(x => x.SalaId != funcion.SalaId)) {
                throw new InvalidOperationException(
                    "Una o más butacas no pertenecen a la sala de la función.");
            }

            var butacasDisponibles =
                await _reservaRepository.ButacasDisponiblesAsync(
                    request.FuncionId,
                    request.ButacasIds,
                    cancellationToken);

            if (!butacasDisponibles) {
                throw new InvalidOperationException(
                    "Una o más butacas ya están reservadas para esta función.");
            }

            var total =
                funcion.Precio * request.ButacasIds.Count;

            var usuarioId = _currentUserService.UserId;

            var reserva = new Reserva(
                usuarioId,
                usuarioId,
                request.FuncionId,
                total);

            await _reservaRepository.AddAsync(
                reserva,
                request.ButacasIds,
                cancellationToken);

            return reserva.Id;
        }
    }
}
