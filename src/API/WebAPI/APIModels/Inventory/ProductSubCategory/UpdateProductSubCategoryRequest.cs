namespace WebAPI.APIModels.Inventory.ProductSubCategory
{
    public record UpdateProductSubCategoryRequest(
        Guid ProductCategoryId,
        string Name,
        string? Description);
}
