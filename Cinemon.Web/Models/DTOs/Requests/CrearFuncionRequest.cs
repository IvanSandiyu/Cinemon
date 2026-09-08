namespace Cinemon.Web.Models.DTOs.Requests
{
    public sealed record CrearFuncionRequest(
        int PeliculaId,
        int SalaId,
        DateTime FechaHoraInicio,
        int Idioma,
        int Formato,
        decimal Precio);
}