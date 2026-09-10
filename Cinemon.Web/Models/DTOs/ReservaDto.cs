namespace Cinemon.Web.Models.DTOs
{
    public sealed record ReservaDto(
        int Id,
        ReservaClienteDto Cliente,
        ReservaFuncionDto Funcion,
        ReservaPeliculaDto Pelicula,
        ReservaSalaDto Sala,
        DateTime FechaReserva,
        string Estado,
        IReadOnlyCollection<ReservaButacaDto> Butacas,
        decimal Total);

    public sealed record ReservaClienteDto(
        string NombreApellido,
        string Email);

    public sealed record ReservaFuncionDto(
        int Id,
        DateTime FechaHoraInicio,
        string Idioma,
        string Formato);

    public sealed record ReservaPeliculaDto(
        string Titulo,
        int Duracion,
        string ClasificacionEdad,
        string PosterUrl);

    public sealed record ReservaSalaDto(
        int Numero,
        string TipoSala);

    public sealed record ReservaButacaDto(
        int Id,
        string Codigo,
        string Fila,
        int Numero);
}