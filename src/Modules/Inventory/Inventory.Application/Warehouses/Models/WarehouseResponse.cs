namespace Inventory.Application.Warehouses.Models
{
    public record WarehouseResponse(
        Guid Id,
        Guid ProductServiceLineId,
        string Name,
        string? Description,
        string? Latitude,
        string? Longitude);
}
