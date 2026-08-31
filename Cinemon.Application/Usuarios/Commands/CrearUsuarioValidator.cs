using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinemon.Application.Usuarios.Commands
{
    public sealed class CrearUsuarioValidator: AbstractValidator<CrearUsuarioCommand>
    {
        public CrearUsuarioValidator()
        {
            RuleFor(x => x.NombreApellido)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.Rol)
                .IsInEnum();
        }
    }
}
