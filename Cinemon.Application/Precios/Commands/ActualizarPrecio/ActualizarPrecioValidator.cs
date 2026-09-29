using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Precios.Commands.ActualizarPrecio
{
    public sealed class ActualizarPrecioValidator: AbstractValidator<ActualizarPrecioCommand>
    {
        public ActualizarPrecioValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Valor)
                .GreaterThan(0)
                .WithMessage("El precio debe ser mayor a 0.");
        }
    }
}
