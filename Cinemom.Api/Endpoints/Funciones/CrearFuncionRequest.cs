using Cinemon.Domain.Enums;

namespace Cinemon.Api.Endpoints.Funciones
{
    public sealed record CrearFuncionRequest(
    int PeliculaId,
    int SalaId,
    DateTime FechaHoraInicio,
    IdiomaFuncion Idioma,
    Formato Formato,
    decimal Precio);
}
