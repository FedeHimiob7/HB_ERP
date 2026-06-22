using Inventory.Domain.Entities;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;

namespace Inventory.Domain.Repositories
{
    public interface IStorageTypeRepository
    {
        Task<StorageType?> GetByIdAsync(StorageTypeId id, CancellationToken cancellationToken = default);
        Task<List<StorageType>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<StorageType> Items, int TotalCount)> GetPagedAsync(StorageTypeFilter filter, CancellationToken cancellationToken = default);
        Task AddAsync(StorageType storageType, CancellationToken cancellationToken = default);
        Task UpdateAsync(StorageType storageType, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, StorageTypeId? excludeId = null, CancellationToken cancellationToken = default);
    }
}
