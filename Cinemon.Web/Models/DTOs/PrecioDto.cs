namespace Cinemon.Web.Models.DTOs
{
    public sealed record PrecioDto(
        int Id,
        int Formato,
        int TipoSala,
        string Nombre,
        decimal Valor);
}
