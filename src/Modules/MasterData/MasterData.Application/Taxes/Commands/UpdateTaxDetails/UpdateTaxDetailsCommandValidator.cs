using FluentValidation;

namespace MasterData.Application.Taxes.Commands.UpdateTaxDetails
{
    public class UpdateTaxDetailsCommandValidator : AbstractValidator<UpdateTaxDetailsCommand>
    {
        public UpdateTaxDetailsCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.TaxType).IsInEnum();
        }
    }
}
