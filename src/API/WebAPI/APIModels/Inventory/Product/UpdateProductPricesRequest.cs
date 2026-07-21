namespace WebAPI.APIModels.Inventory.Product
{
    public record UpdateProductPricesRequest(
        decimal? Cost,
        Guid? CostCurrencyId,
        decimal? CostExchangeRate,
        decimal? Price,
        Guid? PriceCurrencyId,
        decimal? PriceExchangeRate,
        decimal? Price2,
        decimal? Price3,
        decimal? Price4,
        decimal? Price5);
}
