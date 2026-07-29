using ErrorOr;
using MediatR;

namespace Inventory.Application.Products.Queries.CalculatePrices
{
    public record CalculatePricesQuery(
        decimal BaseAmount,
        List<TaxItemQuery> TaxList,
        decimal? Profit,
        decimal? Commission,
        bool IsCost) : IRequest<ErrorOr<PriceCalculationResult>>;
}
