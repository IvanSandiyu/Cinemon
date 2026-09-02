namespace Cinemon.Web.Models.DTOs.Requests
{
    public sealed record CrearReservaRequest(
    int UsuarioId,
    int RealizadaPorId,
    int FuncionId,
    IReadOnlyCollection<int> ButacasIds);
}
