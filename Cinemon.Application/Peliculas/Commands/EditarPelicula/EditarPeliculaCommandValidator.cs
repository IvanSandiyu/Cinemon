using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Peliculas.Commands.EditarPelicula
{
    public class EditarPeliculaCommandValidator : AbstractValidator<EditarPeliculaCommand>
    {
        public EditarPeliculaCommandValidator()
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
