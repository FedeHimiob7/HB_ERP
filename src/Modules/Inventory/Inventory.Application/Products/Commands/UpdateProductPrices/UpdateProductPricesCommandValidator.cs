using FluentValidation;

namespace Inventory.Application.Products.Commands.UpdateProductPrices
{
    public class UpdateProductPricesCommandValidator : AbstractValidator<UpdateProductPricesCommand>
    {
        public UpdateProductPricesCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();

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
