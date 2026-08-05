using FluentValidation;

namespace MasterData.Application.Taxes.Commands.CreateTax
{
    public class CreateTaxCommandValidator : AbstractValidator<CreateTaxCommand>
    {
        public CreateTaxCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.TaxType).IsInEnum();
            RuleFor(x => x.Rate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(1);
        }
    }
}
