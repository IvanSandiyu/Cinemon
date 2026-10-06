using Cinemon.Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinemon.Application.Promociones.Commands.CrearPromocion
{
    public sealed class CrearPromocionValidator
    : AbstractValidator<CrearPromocionCommand>
    {
        public CrearPromocionValidator()
        {
            RuleFor(x => x.Nombre)
                .NotEmpty()
                .WithMessage("El nombre de la promoción es obligatorio.")
                .MaximumLength(100);

            RuleFor(x => x.Descripcion)
                .MaximumLength(500);

            RuleFor(x => x.Tipo)
                .IsInEnum();

            RuleFor(x => x.CantidadPagadas)
                .GreaterThanOrEqualTo(1)
                .When(x => x.Tipo == TipoPromocion.NxM)
                .WithMessage("NxM debe tener al menos 1 entrada a cobrar.");

            RuleFor(x => x.CantidadGratis)
                .GreaterThanOrEqualTo(1)
                .When(x => x.Tipo == TipoPromocion.NxM)
                .WithMessage("NxM debe tener al menos 1 entrada gratis.");

            RuleFor(x => x.PorcentajeDescuento)
                .InclusiveBetween(1,100)
                .When(x => x.Tipo == TipoPromocion.Porcentaje)
                .WithMessage("El descuento debe estar entre 1 y 100.");

            RuleFor(x => x.PorcentajeDescuento)
                .Null()
                .When(x => x.Tipo == TipoPromocion.NxM)
                .WithMessage("NxM no admite porcentaje de descuento.");

            RuleFor(x => x.CantidadPagadas)
                .Null()
                .When(x => x.Tipo == TipoPromocion.Porcentaje)
                .WithMessage("Una promoción por porcentaje no admite cantidades NxM.");

            RuleFor(x => x.CantidadGratis)
                .Null()
                .When(x => x.Tipo == TipoPromocion.Porcentaje)
                .WithMessage("Una promoción por porcentaje no admite cantidades NxM.");

            RuleFor(x => x.FechaDesde)
                .NotEmpty();

            RuleFor(x => x.FechaHasta)
                .NotEmpty()
                .GreaterThanOrEqualTo(x => x.FechaDesde)
                .WithMessage("La fecha de fin no puede ser anterior a la de inicio.");

            RuleFor(x => x.DiasSemana)
                .NotEmpty()
                .WithMessage("Elegí al menos un día de la semana.");

            RuleForEach(x => x.DiasSemana)
                .Must(x => Enum.IsDefined(x))
                .WithMessage("Hay un día de la semana inválido.");
        }
    }
}
