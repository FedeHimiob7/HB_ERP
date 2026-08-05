using FluentValidation;

namespace MasterData.Application.Taxes.Commands.RegisterTaxRate
{
    public class RegisterTaxRateCommandValidator : AbstractValidator<RegisterTaxRateCommand>
    {
        public RegisterTaxRateCommandValidator()
        {
            RuleFor(x => x.TaxId).NotEmpty();
            RuleFor(x => x.Rate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1);
        }
    }
}
