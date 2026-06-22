namespace Inventory.Application.ProductBrands.Models
{
    public record PagedProductBrandsResult(IReadOnlyList<ProductBrandResponse> Items, int TotalCount);
}
