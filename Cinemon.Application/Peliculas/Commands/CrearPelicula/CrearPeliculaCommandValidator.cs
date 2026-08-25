using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Cinemon.Application.Peliculas.Commands.CrearPelicula
{
    public class CrearPeliculaCommandValidator: AbstractValidator<CrearPeliculaCommand>
    {
        //el Validator valida datos de entrada,no valida reglas que requieren consultar la base.
        //El Validator no debería consultar SQL Server para comprobar si 999 existe.
        public CrearPeliculaCommandValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Sinopsis)
                .NotEmpty()
                .MaximumLength(2000);

            RuleFor(x => x.Duracion)
                .GreaterThan(0);

            RuleFor(x => x.FechaEstreno)
                .NotEmpty();

            RuleFor(x => x.PosterUrl)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.TrailerUrl)
                .NotEmpty()
                .MaximumLength(500);

            RuleFor(x => x.GeneroIds)
                .NotEmpty();

            RuleForEach(x => x.GeneroIds)
                .GreaterThan(0);

            RuleFor(x => x.GeneroIds)
          .Must(generos => generos.Distinct().Count() == generos.Count)
          .WithMessage("No se pueden repetir géneros.");
        }
    }
}
