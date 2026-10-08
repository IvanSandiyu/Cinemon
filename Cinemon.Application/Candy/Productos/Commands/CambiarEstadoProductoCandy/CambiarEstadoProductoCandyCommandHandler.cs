using Cinemon.Application.Abstractions;
using Cinemon.Domain.Exceptions;
using MediatR;

namespace Cinemon.Application.Candy.Productos.Commands.CambiarEstadoProductoCandy
{
    public sealed class CambiarEstadoProductoCandyCommandHandler
        : IRequestHandler<CambiarEstadoProductoCandyCommand, Unit>
    {
        private readonly IProductoCandyRepository _productoCandyRepository;

        public CambiarEstadoProductoCandyCommandHandler(
            IProductoCandyRepository productoCandyRepository)
        {
            _productoCandyRepository = productoCandyRepository;
        }

        public async Task<Unit> Handle(
            CambiarEstadoProductoCandyCommand request,
            CancellationToken cancellationToken)
        {
            var producto = await _productoCandyRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (producto is null)
                throw new NotFoundException("El producto no existe.");

            if (request.Activo)
                producto.Activar();
            else
                producto.Desactivar();

            _productoCandyRepository.Update(producto);

            await _productoCandyRepository.GuardarAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
