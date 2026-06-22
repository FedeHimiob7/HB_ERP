using Inventory.Domain.Entities;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Repositories
{
    public interface IWarehouseRepository
    {
        Task<Warehouse?> GetByIdAsync(WarehouseId id, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default);
        Task<List<Warehouse>> GetAllAsync(IReadOnlyList<Guid> allowedPslIds, ProductServiceLineId? productServiceLineId = null, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Warehouse> Items, int TotalCount)> GetPagedAsync(WarehouseFilter filter, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default);
        Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
        Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameInPslAsync(string name, ProductServiceLineId pslId, WarehouseId? excludeId = null, CancellationToken cancellationToken = default);
    }
}
