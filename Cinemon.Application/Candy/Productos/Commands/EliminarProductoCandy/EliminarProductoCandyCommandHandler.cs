using Cinemon.Application.Abstractions;
using Cinemon.Domain.Exceptions;
using MediatR;

namespace Cinemon.Application.Candy.Productos.Commands.EliminarProductoCandy
{
    public sealed class EliminarProductoCandyCommandHandler
        : IRequestHandler<EliminarProductoCandyCommand, Unit>
    {
        private readonly IProductoCandyRepository _productoCandyRepository;

        public EliminarProductoCandyCommandHandler(
            IProductoCandyRepository productoCandyRepository)
        {
            _productoCandyRepository = productoCandyRepository;
        }

        public async Task<Unit> Handle(
            EliminarProductoCandyCommand request,
            CancellationToken cancellationToken)
        {
            var producto = await _productoCandyRepository.ObtenerPorIdAsync(
                request.Id,
                cancellationToken);

            if (producto is null)
                throw new NotFoundException("El producto no existe.");

            if (await _productoCandyRepository.EsComponenteDeAlgunComboAsync(
                request.Id,
                cancellationToken))
            {
                throw new BusinessRuleException(
                    "No se puede eliminar un producto que forma parte de un combo. Desactícalo en su lugar.");
            }

            if (await _productoCandyRepository.TienePedidosAsync(
                request.Id,
                cancellationToken))
            {
                throw new BusinessRuleException(
                    "No se puede eliminar un producto que ya fue vendido. Desactícalo en su lugar.");
            }

            await _productoCandyRepository.EliminarAsync(request.Id, cancellationToken);

            return Unit.Value;
        }
    }
}
