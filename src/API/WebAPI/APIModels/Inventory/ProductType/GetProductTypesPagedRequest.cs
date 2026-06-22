namespace WebAPI.APIModels.Inventory.ProductType
{
    public record GetProductTypesPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null);
}
