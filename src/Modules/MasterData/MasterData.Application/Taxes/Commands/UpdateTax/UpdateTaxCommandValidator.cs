using FluentValidation;

namespace MasterData.Application.Taxes.Commands.UpdateTax
{
    public class UpdateTaxCommandValidator : AbstractValidator<UpdateTaxCommand>
    {
        public UpdateTaxCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.TaxType).IsInEnum();
            RuleFor(x => x.Rate).GreaterThan(0).LessThanOrEqualTo(1);
        }
    }
}
