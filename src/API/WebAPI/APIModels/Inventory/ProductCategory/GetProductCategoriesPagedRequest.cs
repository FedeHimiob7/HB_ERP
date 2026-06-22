namespace WebAPI.APIModels.Inventory.ProductCategory
{
    public record GetProductCategoriesPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        Guid? ProductServiceLineId = null,
        Guid? ProductTypeId = null);
}
