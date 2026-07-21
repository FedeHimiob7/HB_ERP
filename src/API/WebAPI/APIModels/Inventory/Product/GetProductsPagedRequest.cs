namespace WebAPI.APIModels.Inventory.Product
{
    public record GetProductsPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        Guid? ProductServiceLineId = null,
        Guid? ProductTypeId = null,
        Guid? ProductCategoryId = null,
        Guid? ProductBrandId = null,
        bool? IsSalable = null,
        bool? IsPurchasable = null);
}
