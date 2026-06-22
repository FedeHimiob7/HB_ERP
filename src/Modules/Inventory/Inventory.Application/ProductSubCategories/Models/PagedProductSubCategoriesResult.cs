namespace Inventory.Application.ProductSubCategories.Models
{
    public record PagedProductSubCategoriesResult(IReadOnlyList<ProductSubCategoryResponse> Items, int TotalCount);
}
