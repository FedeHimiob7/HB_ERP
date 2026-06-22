namespace WebAPI.APIModels.Inventory.StorageType
{
    public record GetStorageTypesPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null);
}
