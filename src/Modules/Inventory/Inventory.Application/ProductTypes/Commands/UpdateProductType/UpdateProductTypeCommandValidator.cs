using FluentValidation;

namespace Inventory.Application.ProductTypes.Commands.UpdateProductType
{
    public class UpdateProductTypeCommandValidator : AbstractValidator<UpdateProductTypeCommand>
    {
        public UpdateProductTypeCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
        }
    }
}
