namespace WebAPI.APIModels.Inventory.Warehouse
{
    public record UpdateWarehouseRequest(
        Guid ProductServiceLineId,
        string Name,
        string? Description,
        string? Latitude,
        string? Longitude);
}
