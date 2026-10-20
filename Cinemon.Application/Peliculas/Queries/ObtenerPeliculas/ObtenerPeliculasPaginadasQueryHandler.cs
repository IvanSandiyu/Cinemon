using Cinemon.Application.Abstractions;
using Cinemon.Application.Common;
using MediatR;

namespace Cinemon.Application.Peliculas.Queries.ObtenerPeliculas
{
    public sealed class ObtenerPeliculasPaginadasQueryHandler
        : IRequestHandler<ObtenerPeliculasPaginadasQuery, PagedResult<PeliculaDto>>
    {
        private const int TamanoPaginaMaximo = 50;

        private readonly IPeliculaRepository _peliculaRepository;
        private readonly TimeProvider _timeProvider;

        public ObtenerPeliculasPaginadasQueryHandler(
            IPeliculaRepository peliculaRepository,
            TimeProvider timeProvider)
        {
            _peliculaRepository = peliculaRepository;
            _timeProvider = timeProvider;
        }

        public async Task<PagedResult<PeliculaDto>> Handle(
            ObtenerPeliculasPaginadasQuery request,
            CancellationToken cancellationToken)
        {
            var pagina = Math.Max(1, request.Pagina);
            var tamanoPagina = Math.Clamp(
                request.TamanoPagina,
                1,
                TamanoPaginaMaximo);

            var busqueda = string.IsNullOrWhiteSpace(request.Busqueda)
                ? null
                : request.Busqueda.Trim();

            var hoy = _timeProvider.Ahora().Date;

            var (peliculas, total) = await _peliculaRepository
                .ObtenerPaginadasAsync(
                    request.Filtro,
                    busqueda,
                    request.Ids ?? [],
                    pagina,
                    tamanoPagina,
                    hoy,
                    cancellationToken);

            var items = peliculas
                .Select(PeliculaDtoMapper.ToDto)
                .ToList();

            return new PagedResult<PeliculaDto>(
                items,
                pagina,
                tamanoPagina,
                total);
        }
    }
}
