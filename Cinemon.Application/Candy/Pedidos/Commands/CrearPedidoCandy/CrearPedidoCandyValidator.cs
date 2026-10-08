using Cinemon.Domain.Exceptions;
using FluentValidation;

namespace Cinemon.Application.Candy.Pedidos.Commands.CrearPedidoCandy
{
    public sealed class CrearPedidoCandyValidator
        : AbstractValidator<CrearPedidoCandyCommand>
    {
        public CrearPedidoCandyValidator()
        {
            RuleFor(x => x.Items)
                .NotNull()
                .WithMessage("El pedido debe tener al menos un producto.")
                .Must(items => items.Count > 0)
                .WithMessage("El pedido debe tener al menos un producto.");

            RuleFor(x => x.Items)
                .Must(items => items is null || items.Count <= 50)
                .WithMessage("El pedido no puede superar los 50 productos distintos.");

            RuleForEach(x => x.Items)
                .Must(item => item.ProductoId > 0)
                .WithMessage("El pedido tiene un producto inválido.");

            RuleForEach(x => x.Items)
                .Must(item => item.Cantidad > 0)
                .WithMessage("La cantidad de cada producto debe ser al menos 1.");

            RuleFor(x => x.Items)
                .Must(items => items is null || items.All(x => x.Cantidad <= 100))
                .WithMessage("La cantidad máxima por producto es 100.");
        }
    }
}
