using Cinemon.Application.Abstractions;
using Cinemon.Domain.Enums;
using MediatR;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed class ObtenerReservasQueryHandler: IRequestHandler<ObtenerReservasQuery, IReadOnlyCollection<ReservaDto>>
    {
        private readonly IReservaRepository _reservaRepository;

        public ObtenerReservasQueryHandler(IReservaRepository reservaRepository)
        {
            _reservaRepository = reservaRepository;
        }

        public Task<IReadOnlyCollection<ReservaDto>> Handle(ObtenerReservasQuery request,CancellationToken cancellationToken)
        {
            return _reservaRepository.ObtenerReservasAsync(usuarioId: null, cancellationToken);
        }
    }

    internal static class ReservaMapper
    {
        public static async Task<ReservaDto> ToDtoAsync(
            Domain.Entidades.Reservas.Reserva reserva,
            IUsuarioRepository usuarioRepository,
            IFuncionRepository funcionRepository,
            IPeliculaRepository peliculaRepository,
            ISalaRepository salaRepository,
            IButacaRepository butacaRepository,
            IReservaRepository reservaRepository,
            CancellationToken cancellationToken)
        {
            var cliente = await usuarioRepository
                .ObtenerPorIdAsync(reserva.UsuarioId, cancellationToken);

            var funcion = await funcionRepository
                .ObtenerPorIdAsync(reserva.FuncionId, cancellationToken);

            var pelicula = funcion is null
                ? null
                : await peliculaRepository
                    .ObtenerPorIdAsync(funcion.PeliculaId, cancellationToken);

            var sala = funcion is null
                ? null
                : await salaRepository
                    .ObtenerPorIdAsync(funcion.SalaId, cancellationToken);

            var butacaIds = await reservaRepository
                .ObtenerButacaIdsAsync(reserva.Id, cancellationToken);

            var butacas = butacaIds.Count == 0
                ? Array.Empty<Domain.Entidades.Butacas.Butaca>()
                : await butacaRepository.ObtenerPorIdsAsync(butacaIds, cancellationToken);

            return new ReservaDto(
                reserva.Id,
                cliente is null
                    ? new ClienteDto("", "")
                    : new ClienteDto(cliente.NombreApellido, cliente.Email),
                funcion is null
                    ? new FuncionDto(reserva.FuncionId, default, "", "")
                    : new FuncionDto(
                        funcion.Id,
                        funcion.FechaHoraInicio,
                        funcion.Idioma.ToString(),
                        funcion.Formato.ToString()),
                pelicula is null
                    ? new PeliculaDto("", 0, "", "")
                    : new PeliculaDto(
                        pelicula.Titulo,
                        pelicula.Duracion,
                        pelicula.ClasificacionEdad.ToString(),
                        pelicula.PosterUrl),
                sala is null
                    ? new SalaDto(0, "")
                    : new SalaDto(sala.Numero, sala.TipoSala.ToString()),
                reserva.FechaReserva,
                reserva.EstadoReserva.ToString(),
                butacas
                    .Select(x => new ButacaDto(x.Id, x.Codigo, x.Fila, x.Numero))
                    .ToList(),
                reserva.Total);
        }
    }
}

