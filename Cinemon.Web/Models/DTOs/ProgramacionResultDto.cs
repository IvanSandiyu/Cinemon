namespace Cinemon.Web.Models.DTOs
{
    public sealed record ProgramacionResultDto(
        IReadOnlyCollection<ProgramacionFuncionDto> Creadas,
        IReadOnlyCollection<DateTime> Omitidas);

    public sealed record ProgramacionFuncionDto(
        int Id,
        DateTime FechaHoraInicio);
}
