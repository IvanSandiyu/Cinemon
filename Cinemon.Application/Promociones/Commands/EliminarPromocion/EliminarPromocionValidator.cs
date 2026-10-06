using FluentValidation;

namespace Cinemon.Application.Promociones.Commands.EliminarPromocion
{
    public sealed class EliminarPromocionValidator
    : AbstractValidator<EliminarPromocionCommand>
    {
        public EliminarPromocionValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
