namespace WebAPI.APIModels.Inventory.ProductCategory
{
    public record CreateProductCategoryRequest(
        Guid ProductServiceLineId,
        Guid? ProductTypeId,
        string Name,
        string? Description);
}
