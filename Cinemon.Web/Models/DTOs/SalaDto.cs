namespace Cinemon.Web.Models.DTOs
{
    public sealed record SalaDto(
        int Id,
        int Numero,
        string TipoSala,
        int CantidadButacas);
}