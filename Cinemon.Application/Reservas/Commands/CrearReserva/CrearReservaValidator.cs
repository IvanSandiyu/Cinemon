using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Reservas.Commands.CrearReserva
{
    public sealed class CrearReservaValidator: AbstractValidator<CrearReservaCommand>
    {
        public CrearReservaValidator()
        {
          
            RuleFor(x => x.FuncionId)
                .GreaterThan(0);

            RuleFor(x => x.ButacasIds)
                .NotEmpty()
                .WithMessage("Debe seleccionar al menos una butaca.");

            RuleFor(x => x.ButacasIds)
                .Must(x => x.Distinct().Count() == x.Count)
                .WithMessage("No se pueden seleccionar butacas repetidas.");
        }
    }
}
