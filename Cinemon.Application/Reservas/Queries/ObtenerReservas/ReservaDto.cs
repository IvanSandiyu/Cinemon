using Cinemon.Domain.Enums;

namespace Cinemon.Application.Reservas.Queries.ObtenerReservas
{
    public sealed record ReservaDto(
        int Id,
        ClienteDto Cliente,
        FuncionDto Funcion,
        PeliculaDto Pelicula,
        SalaDto Sala,
        DateTime FechaReserva,
        string Estado,
        IReadOnlyCollection<ButacaDto> Butacas,
        decimal Total);

    public sealed record ClienteDto(
        string NombreApellido,
        string Email);

    public sealed record FuncionDto(
        int Id,
        DateTime FechaHoraInicio,
        string Idioma,
        string Formato);

    public sealed record PeliculaDto(
        string Titulo,
        int Duracion,
        string ClasificacionEdad,
        string PosterUrl);

    public sealed record SalaDto(
        int Numero,
        string TipoSala);

    public sealed record ButacaDto(
        int Id,
        string Codigo,
        string Fila,
        int Numero);
}
