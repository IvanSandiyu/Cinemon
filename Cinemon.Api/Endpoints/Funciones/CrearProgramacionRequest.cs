using Cinemon.Domain.Enums;

namespace Cinemon.Api.Endpoints.Funciones
{
    public sealed record CrearProgramacionRequest(
     int PeliculaId,
     int SalaId,
     DateTime FechaInicio,
     IReadOnlyCollection<int> DiasSemana,
     int CantidadSemanas,
     IdiomaFuncion Idioma,
     Formato Formato,
     decimal Precio);

    public sealed record ProgramacionResponse(
     IReadOnlyCollection<ProgramacionItemResponse> Creadas,
     IReadOnlyCollection<DateTime> Omitidas);

    public sealed record ProgramacionItemResponse(int Id,DateTime FechaHoraInicio);
}
