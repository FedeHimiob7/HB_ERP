namespace Inventory.Application.StorageTypes.Models
{
    public record PagedStorageTypesResult(IReadOnlyList<StorageTypeResponse> Items, int TotalCount);
}
