namespace WebAPI.APIModels.Inventory.ProductSubCategory
{
    public record GetProductSubCategoriesPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        Guid? ProductCategoryId = null);
}
