using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Funciones.Commands.CrearFuncion
{
    public sealed class CrearFuncionValidator: AbstractValidator<CrearFuncionCommand>
    {
        public CrearFuncionValidator()
        {
            RuleFor(x => x.PeliculaId)
                .GreaterThan(0);

            RuleFor(x => x.SalaId)
                .GreaterThan(0);

            RuleFor(x => x.FechaHoraInicio)
                .GreaterThan(DateTime.Now);

            RuleFor(x => x.Precio)
                .GreaterThan(0);
        }
    }
}
