namespace Inventory.Application.ProductCategories.Models
{
    public record ProductCategoryResponse(
        Guid Id,
        Guid ProductServiceLineId,
        Guid? ProductTypeId,
        string Name,
        string? Description);
}
