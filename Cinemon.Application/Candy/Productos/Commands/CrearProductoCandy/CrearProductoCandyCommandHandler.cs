using Cinemon.Application.Abstractions;
using Cinemon.Domain.Entidades.Candy;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Cinemon.Application.Candy.Productos.Commands.CrearProductoCandy
{
    public sealed class CrearProductoCandyCommandHandler
        : IRequestHandler<CrearProductoCandyCommand, int>
    {
        private readonly IProductoCandyRepository _productoCandyRepository;

        public CrearProductoCandyCommandHandler(
            IProductoCandyRepository productoCandyRepository)
        {
            _productoCandyRepository = productoCandyRepository;
        }

        public async Task<int> Handle(
            CrearProductoCandyCommand request,
            CancellationToken cancellationToken)
        {
            var componentes = await ComponentesComboHelper.ResolverAsync(
                _productoCandyRepository,
                request.Categoria,
                request.Componentes,
                cancellationToken);

            var producto = new ProductoCandy(
                request.Nombre,
                request.Descripcion,
                request.Precio,
                request.Categoria,
                componentes.Items);

            producto.ValidarPrecioCombo(componentes.Suma);

            await _productoCandyRepository.AddAsync(producto, cancellationToken);

            return producto.Id;
        }
    }
}
