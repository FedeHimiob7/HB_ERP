using FluentValidation;

namespace Inventory.Application.ProductSubCategories.Commands.CreateProductSubCategory
{
    public class CreateProductSubCategoryCommandValidator : AbstractValidator<CreateProductSubCategoryCommand>
    {
        public CreateProductSubCategoryCommandValidator()
        {
            RuleFor(x => x.ProductCategoryId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
        }
    }
}
