namespace Inventory.Application.ProductSubCategories.Models
{
    public record ProductSubCategoryResponse(Guid Id, Guid ProductCategoryId, string Name, string? Description);
}
