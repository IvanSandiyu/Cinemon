using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Funcion;
using Cinemon.Domain.Enums;
using Cinemon.Domain.Exceptions;
using MediatR;

namespace Cinemon.Application.Funciones.Commands.CrearProgramacion
{
    public sealed class CrearProgramacionCommandHandler : IRequestHandler<CrearProgramacionCommand,CrearProgramacionResult>
    {
        private readonly IPeliculaRepository _peliculaRepository;
        private readonly ISalaRepository _salaRepository;
        private readonly IFuncionRepository _funcionRepository;

        public CrearProgramacionCommandHandler(IPeliculaRepository peliculaRepository,ISalaRepository salaRepository,
            IFuncionRepository funcionRepository)
        {
            _peliculaRepository = peliculaRepository;
            _salaRepository = salaRepository;
            _funcionRepository = funcionRepository;
        }

        public async Task<CrearProgramacionResult> Handle(CrearProgramacionCommand request,
            CancellationToken cancellationToken)
        {
            var pelicula = await _peliculaRepository.ObtenerPorIdAsync(
                request.PeliculaId,
                cancellationToken);

            if (pelicula is null) {
                throw new NotFoundException(
                    "La película no existe.");
            }

            if (!pelicula.Activa) {
                throw new BusinessRuleException(
                    "Solo se pueden programar funciones de películas activas.");
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

            var fechas = ConstruirFechas(
                request.FechaInicio,
                request.DiasSemana,
                request.CantidadSemanas);

            if (fechas.Count == 0) {
                throw new BusinessRuleException(
                    "La selección no genera ninguna fecha para programar.");
            }

            var creadas = new List<FuncionProgramada>();
            var omitidas = new List<DateTime>();

            foreach (var fechaHoraInicio in fechas) {
                cancellationToken.ThrowIfCancellationRequested();

                if (fechaHoraInicio <= DateTime.UtcNow) {
                    omitidas.Add(fechaHoraInicio);
                    continue;
                }

                var fechaHoraFin = fechaHoraInicio
                    .AddMinutes(pelicula.Duracion);

                var existeSuperposicion =
                    await _funcionRepository.ExisteSuperposicionAsync(
                        request.SalaId,
                        fechaHoraInicio,
                        fechaHoraFin,
                        cancellationToken);

                if (existeSuperposicion) {
                    omitidas.Add(fechaHoraInicio);
                    continue;
                }

                var funcion = new Funcion(
                    request.PeliculaId,
                    request.SalaId,
                    fechaHoraInicio,
                    request.Idioma,
                    request.Formato,
                    request.Precio);

                await _funcionRepository.AddAsync(
                    funcion,
                    cancellationToken);

                creadas.Add(new FuncionProgramada(
                    funcion.Id,
                    fechaHoraInicio));
            }

            return new CrearProgramacionResult(creadas,omitidas);
        }

        private static List<DateTime> ConstruirFechas(DateTime fechaInicio,
            IReadOnlyCollection<DayOfWeek> diasSemana,
            int cantidadSemanas)
        {
            var inicio = fechaInicio.Date;
            var hora = fechaInicio.TimeOfDay;

            var fechas = new List<DateTime>();

            foreach (var dia in diasSemana.Distinct()) {
                var diasDeEspera = ((int)dia - (int)inicio.DayOfWeek + 7) % 7;

                var primerDia = inicio.AddDays(diasDeEspera);

                for (var semana = 0; semana < cantidadSemanas; semana++) {
                    var fecha = primerDia.AddDays(semana * 7);

                    if (!fechas.Contains(fecha))
                        fechas.Add(fecha.Add(hora));
                }
            }

            fechas.Sort();

            return fechas;
        }
    }
}
