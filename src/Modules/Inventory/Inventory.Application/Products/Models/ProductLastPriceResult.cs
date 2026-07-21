namespace Inventory.Application.Products.Models
{
    public record ProductLastPriceResult(
        decimal? OldCost,
        Guid? OldCostCurrencyId,
        decimal? OldCostExchangeRate,
        decimal? OldPrice,
        Guid? OldPriceCurrencyId,
        decimal? OldPriceExchangeRate,
        decimal? OldPrice2,
        decimal? OldPrice3,
        decimal? OldPrice4,
        decimal? OldPrice5,
        DateTime ChangedAt);
}
