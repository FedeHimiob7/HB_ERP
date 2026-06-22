using FluentValidation;

namespace Inventory.Application.ProductTypes.Commands.CreateProductType
{
    public class CreateProductTypeCommandValidator : AbstractValidator<CreateProductTypeCommand>
    {
        public CreateProductTypeCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
        }
    }
}
