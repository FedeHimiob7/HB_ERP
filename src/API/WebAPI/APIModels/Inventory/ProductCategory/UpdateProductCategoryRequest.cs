namespace WebAPI.APIModels.Inventory.ProductCategory
{
    public record UpdateProductCategoryRequest(
        Guid ProductServiceLineId,
        Guid? ProductTypeId,
        string Name,
        string? Description);
}
