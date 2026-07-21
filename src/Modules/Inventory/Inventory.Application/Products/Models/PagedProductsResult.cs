namespace Inventory.Application.Products.Models
{
    public record PagedProductsResult(IReadOnlyList<ProductResponse> Items, int TotalCount);
}
