namespace WebAPI.APIModels.Inventory.Product
{
    public record UpdateProductPricesRequest(
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
        decimal? ProfitMargin);
}
