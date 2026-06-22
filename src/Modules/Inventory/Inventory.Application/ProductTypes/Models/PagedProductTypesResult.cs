namespace Inventory.Application.ProductTypes.Models
{
    public record PagedProductTypesResult(IReadOnlyList<ProductTypeResponse> Items, int TotalCount);
}
