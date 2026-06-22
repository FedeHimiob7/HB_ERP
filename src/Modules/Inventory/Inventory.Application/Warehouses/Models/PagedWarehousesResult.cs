namespace Inventory.Application.Warehouses.Models
{
    public record PagedWarehousesResult(IReadOnlyList<WarehouseResponse> Items, int TotalCount);
}
