using Cinemon.Application.Common;
using MediatR;

namespace Cinemon.Application.Peliculas.Queries.ObtenerPeliculas
{
    public sealed record ObtenerPeliculasPaginadasQuery(
        int Pagina,
        int TamanoPagina,
        FiltroPeliculasPublicas Filtro,
        string? Busqueda = null,
        IReadOnlyCollection<int>? Ids = null)
        : IRequest<PagedResult<PeliculaDto>>;
}
