using FluentValidation;

namespace Inventory.Application.ProductBrands.Commands.CreateProductBrand
{
    public class CreateProductBrandCommandValidator : AbstractValidator<CreateProductBrandCommand>
    {
        public CreateProductBrandCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
        }
    }
}
