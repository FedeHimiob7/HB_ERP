using FluentValidation;

namespace Inventory.Application.ProductSubCategories.Commands.UpdateProductSubCategory
{
    public class UpdateProductSubCategoryCommandValidator : AbstractValidator<UpdateProductSubCategoryCommand>
    {
        public UpdateProductSubCategoryCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.ProductCategoryId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Description).MaximumLength(250).When(x => x.Description != null);
        }
    }
}
