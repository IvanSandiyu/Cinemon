using Cinemon.Domain.Enums;
using FluentValidation;
using System.Collections.Generic;

namespace Cinemon.Application.Candy.Productos.Commands.CrearProductoCandy
{
    public sealed class CrearProductoCandyValidator
        : AbstractValidator<CrearProductoCandyCommand>
    {
        public CrearProductoCandyValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El producto debe tener un nombre.")
                .MaximumLength(100)
                .WithMessage("El nombre del producto no puede superar los 100 caracteres.");

            RuleFor(x => x.Descripcion)
                .MaximumLength(500)
                .WithMessage("La descripción no puede superar los 500 caracteres.");

            RuleFor(x => x.Precio)
                .GreaterThan(0)
                .WithMessage("El precio del producto debe ser mayor a cero.");

            RuleFor(x => x.Categoria)
                .IsInEnum()
                .WithMessage("La categoría del producto no es válida.");

            RuleFor(x => x.Componentes)
                .Must(c => c is not null && c.Count > 0)
                .When(x => x.Categoria == CategoriaCandy.Combo)
                .WithMessage("Un combo debe tener al menos un producto.");

            RuleFor(x => x.Componentes)
                .Must(c => c is null || c.Count == 0)
                .When(x => x.Categoria != CategoriaCandy.Combo)
                .WithMessage("Solo los combos pueden tener componentes.");

            RuleForEach(x => x.Componentes)
                .Must(componente => componente.ProductoId > 0)
                .WithMessage("El combo tiene un producto inválido.");

            RuleForEach(x => x.Componentes)
                .Must(componente => componente.Cantidad > 0)
                .WithMessage("La cantidad de cada componente debe ser al menos 1.");
        }
    }
}
