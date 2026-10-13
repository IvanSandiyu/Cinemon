using Cinemon.Application.Common;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.EditarFuncion
{
    public sealed class EditarFuncionValidator: AbstractValidator<EditarFuncionCommand>
    {
        public EditarFuncionValidator(TimeProvider reloj)
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.PeliculaId)
                .GreaterThan(0);

            RuleFor(x => x.SalaId)
                .GreaterThan(0);

            RuleFor(x => x.FechaHoraInicio)
                .Must(f => f > reloj.Ahora())
                .WithMessage("La fecha de inicio debe ser futura.");

            RuleFor(x => x.Precio)
                .GreaterThan(0);

            RuleFor(x => x.Idioma)
                .IsInEnum();

            RuleFor(x => x.Formato)
                .IsInEnum();
        }
    }
}
