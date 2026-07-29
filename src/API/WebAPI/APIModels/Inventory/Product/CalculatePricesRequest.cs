using Inventory.Application.Products.Queries.CalculatePrices;

namespace WebAPI.APIModels.Inventory.Product
{
    public record CalculatePricesRequest(
        decimal BaseAmount,
        List<TaxItemRequest> TaxList,
        decimal? Profit,
        decimal? Commission,
        bool IsCost);

    public record TaxItemRequest(Guid TaxId, decimal Rate, bool IsIGTF);
}
