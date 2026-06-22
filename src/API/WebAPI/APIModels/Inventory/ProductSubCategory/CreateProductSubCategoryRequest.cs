namespace WebAPI.APIModels.Inventory.ProductSubCategory
{
    public record CreateProductSubCategoryRequest(
        Guid ProductCategoryId,
        string Name,
        string? Description);
}
