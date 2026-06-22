using FluentValidation;

namespace Inventory.Application.ProductBrands.Commands.UpdateProductBrand
{
    public class UpdateProductBrandCommandValidator : AbstractValidator<UpdateProductBrandCommand>
    {
        public UpdateProductBrandCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
        }
    }
}
