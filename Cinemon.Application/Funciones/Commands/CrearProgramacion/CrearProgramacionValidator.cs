using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.CrearProgramacion
{
    public sealed class CrearProgramacionValidator: AbstractValidator<CrearProgramacionCommand>
    {
        public CrearProgramacionValidator()
        {
            RuleFor(x => x.PeliculaId)
                .GreaterThan(0);

            RuleFor(x => x.SalaId)
                .GreaterThan(0);

            RuleFor(x => x.FechaInicio)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("La fecha y hora de inicio debe estar en el futuro.");

            RuleFor(x => x.CantidadSemanas)
                .InclusiveBetween(1,12)
                .WithMessage("La cantidad de semanas debe estar entre 1 y 12.");

            RuleFor(x => x.Precio)
                .GreaterThan(0);

            RuleFor(x => x.DiasSemana)
                .NotEmpty()
                .WithMessage("Seleccioná al menos un día de la semana.")
                .Must(d => d is null || d.Distinct().Count() == d.Count)
                .WithMessage("No se pueden repetir días de la semana.");
        }
    }
}
