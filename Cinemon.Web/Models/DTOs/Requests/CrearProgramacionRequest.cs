namespace Cinemon.Web.Models.DTOs.Requests
{
    public sealed record CrearProgramacionRequest(
        int PeliculaId,
        int SalaId,
        DateTime FechaInicio,
        IReadOnlyCollection<int> DiasSemana,
        int CantidadSemanas,
        int Idioma,
        int Formato,
        decimal Precio);
}
