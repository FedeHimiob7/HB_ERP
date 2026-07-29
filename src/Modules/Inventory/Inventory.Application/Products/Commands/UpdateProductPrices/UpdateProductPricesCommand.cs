using ErrorOr;
using Inventory.Application.Products.Models;
using MediatR;

namespace Inventory.Application.Products.Commands.UpdateProductPrices
{
    public record UpdateProductPricesCommand(
        Guid Id,
        decimal? Cost,
        decimal? CostBase,
        Guid? CostCurrencyId,
        decimal? CostExchangeRate,
        decimal? Price,
        decimal? PriceBase,
        Guid? PriceCurrencyId,
        decimal? PriceExchangeRate,
        decimal? Price2,
        decimal? Price3,
        decimal? Price4,
        decimal? Price5,
        List<Guid>? PurchaseTaxIds,
        List<Guid>? SaleTaxIds,
        decimal? ProfitMargin) : IRequest<ErrorOr<ProductResponse>>;
}
