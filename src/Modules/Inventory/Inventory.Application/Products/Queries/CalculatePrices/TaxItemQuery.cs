namespace Inventory.Application.Products.Queries.CalculatePrices
{
    public record TaxItemQuery(Guid TaxId, decimal Rate, bool IsIGTF);
}
