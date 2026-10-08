using Cinemon.Application.Abstractions;
using Cinemon.Domain.Exceptions;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Cinemon.Application.Candy.Productos.Commands.ActualizarProductoCandy
{
    public sealed class ActualizarProductoCandyCommandHandler
        : IRequestHandler<ActualizarProductoCandyCommand, Unit>
    {
        private readonly IProductoCandyRepository _productoCandyRepository;

        public ActualizarProductoCandyCommandHandler(
            IProductoCandyRepository productoCandyRepository)
        {
            _productoCandyRepository = productoCandyRepository;
        }

        public async Task<Unit> Handle(
            ActualizarProductoCandyCommand request,
            CancellationToken cancellationToken)
        {
            var producto = await _productoCandyRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (producto is null)
                throw new NotFoundException("El producto no existe.");

            var componentes = await ComponentesComboHelper.ResolverAsync(
                _productoCandyRepository,
                request.Categoria,
                request.Componentes,
                cancellationToken);

            producto.Actualizar(
                request.Nombre,
                request.Descripcion,
                request.Precio,
                request.Categoria,
                componentes.Items);

            producto.ValidarPrecioCombo(componentes.Suma);

            _productoCandyRepository.Update(producto);

            await _productoCandyRepository.GuardarAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
