using FluentValidation;

namespace Cinemon.Application.Promociones.Commands.CambiarEstadoPromocion
{
    public sealed class CambiarEstadoPromocionValidator
    : AbstractValidator<CambiarEstadoPromocionCommand>
    {
        public CambiarEstadoPromocionValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
