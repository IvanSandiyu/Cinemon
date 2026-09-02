namespace Cinemon.Web.Models.DTOs
{
    public sealed record FuncionDto(
    int Id,
    int PeliculaId,
    int SalaId,
    DateTime FechaHoraInicio,
    int Idioma,
    int Formato,
    decimal Precio,
    int EstadoFuncion);
}
