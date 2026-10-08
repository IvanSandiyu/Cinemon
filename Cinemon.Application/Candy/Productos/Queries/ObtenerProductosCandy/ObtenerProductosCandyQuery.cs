using Cinemon.Application.DTOs.Candy;
using Cinemon.Application.Abstractions;
using MediatR;

namespace Cinemon.Application.Candy.Productos.Queries.ObtenerProductosCandy
{
    public sealed record ObtenerProductosCandyQuery(bool IncluirInactivos = false)
        : IRequest<IReadOnlyCollection<ProductoCandyDto>>;

    public sealed class ObtenerProductosCandyQueryHandler
        : IRequestHandler<ObtenerProductosCandyQuery, IReadOnlyCollection<ProductoCandyDto>>
    {
        private readonly IProductoCandyRepository _productoCandyRepository;

        public ObtenerProductosCandyQueryHandler(
            IProductoCandyRepository productoCandyRepository)
        {
            _productoCandyRepository = productoCandyRepository;
        }

        public async Task<IReadOnlyCollection<ProductoCandyDto>> Handle(
            ObtenerProductosCandyQuery request,
            CancellationToken cancellationToken)
        {
            var productos = request.IncluirInactivos
                ? await _productoCandyRepository.ObtenerTodasAsync(cancellationToken)
                : await _productoCandyRepository.ObtenerActivasAsync(cancellationToken);

            return productos.Select(x => new ProductoCandyDto(
                x.Id,
                x.Nombre,
                x.Descripcion,
                x.Precio,
                (int)x.Categoria,
                x.Activo,
                x.Componentes
                    .OrderBy(c => c.Componente!.Nombre)
                    .Select(c => new ComponenteComboDto(
                        c.ComponenteProductoId,
                        c.Componente!.Nombre,
                        c.Cantidad,
                        c.Componente.Precio))
                    .ToList()))
                .ToList();
        }
    }
}
