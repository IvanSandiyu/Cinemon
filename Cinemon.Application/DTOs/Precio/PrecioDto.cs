namespace Cinemon.Application.DTOs.Precio
{
    public sealed record PrecioDto(
        int Id,
        int Formato,
        int TipoSala,
        string Nombre,
        decimal Valor);
}
