using FluentValidation;

namespace Inventory.Application.Products.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Name).NotEmpty().MaximumLength(70);
            RuleFor(x => x.ProductServiceLineId).NotEmpty();
            RuleFor(x => x.Description).MaximumLength(700).When(x => x.Description != null);
            RuleFor(x => x.Model).MaximumLength(75).When(x => x.Model != null);
            RuleFor(x => x.Barcode).MaximumLength(50).When(x => x.Barcode != null);
            RuleFor(x => x.ClientCode).MaximumLength(50).When(x => x.ClientCode != null);

            // Bloque costo: todo o nada
            When(x => x.Cost.HasValue || x.CostCurrencyId.HasValue || x.CostExchangeRate.HasValue, () =>
            {
                RuleFor(x => x.Cost).NotNull().GreaterThanOrEqualTo(0);
                RuleFor(x => x.CostCurrencyId).NotNull().NotEmpty();
                RuleFor(x => x.CostExchangeRate).NotNull().GreaterThan(0);
            });

            // Bloque precio: todo o nada
            When(x => x.Price.HasValue || x.PriceCurrencyId.HasValue || x.PriceExchangeRate.HasValue, () =>
            {
                RuleFor(x => x.Price).NotNull().GreaterThanOrEqualTo(0);
                RuleFor(x => x.PriceCurrencyId).NotNull().NotEmpty();
                RuleFor(x => x.PriceExchangeRate).NotNull().GreaterThan(0);
            });
        }
    }
}
