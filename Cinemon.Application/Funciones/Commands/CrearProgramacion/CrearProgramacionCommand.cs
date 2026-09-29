using Cinemon.Domain.Enums;
using MediatR;

namespace Cinemon.Application.Funciones.Commands.CrearProgramacion
{
    public sealed record CrearProgramacionCommand(
     int PeliculaId,
     int SalaId,
     DateTime FechaInicio,
     IReadOnlyCollection<DayOfWeek> DiasSemana,
     int CantidadSemanas,
     IdiomaFuncion Idioma,
     Formato Formato,
     decimal Precio) : IRequest<CrearProgramacionResult>;

    public sealed record FuncionProgramada(int Id,DateTime FechaHoraInicio);

    public sealed record CrearProgramacionResult(
     IReadOnlyCollection<FuncionProgramada> Creadas,
     IReadOnlyCollection<DateTime> Omitidas);
}
