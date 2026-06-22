namespace WebAPI.APIModels.Inventory.Warehouse
{
    public record GetWarehousesPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        Guid? ProductServiceLineId = null);
}
