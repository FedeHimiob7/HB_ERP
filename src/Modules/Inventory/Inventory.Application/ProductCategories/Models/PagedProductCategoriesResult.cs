namespace Inventory.Application.ProductCategories.Models
{
    public record PagedProductCategoriesResult(IReadOnlyList<ProductCategoryResponse> Items, int TotalCount);
}
