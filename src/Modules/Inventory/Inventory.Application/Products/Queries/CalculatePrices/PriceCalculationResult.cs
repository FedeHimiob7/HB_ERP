namespace Inventory.Application.Products.Queries.CalculatePrices
{
    public record PriceCalculationResult(
        decimal Cost,
        decimal Price1,
        decimal Price2,
        decimal Price3,
        decimal Price4,
        decimal Price5);
}
