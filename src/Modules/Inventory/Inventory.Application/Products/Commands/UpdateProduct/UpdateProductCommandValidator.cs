using FluentValidation;

namespace Inventory.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(70);
            RuleFor(x => x.Code).MaximumLength(50).When(x => x.Code != null);
            RuleFor(x => x.Description).MaximumLength(700).When(x => x.Description != null);
            RuleFor(x => x.Model).MaximumLength(75).When(x => x.Model != null);
            RuleFor(x => x.Barcode).MaximumLength(50).When(x => x.Barcode != null);
            RuleFor(x => x.ClientCode).MaximumLength(50).When(x => x.ClientCode != null);
        }
    }
}
