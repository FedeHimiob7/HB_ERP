namespace WebAPI.APIModels.Inventory.Warehouse
{
    public record CreateWarehouseRequest(
        Guid ProductServiceLineId,
        string Name,
        string? Description,
        string? Latitude,
        string? Longitude);
}
