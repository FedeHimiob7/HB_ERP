using Inventory.Domain.Entities;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Repositories
{
    public interface IProductCategoryRepository
    {
        Task<ProductCategory?> GetByIdAsync(ProductCategoryId id, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default);
        Task<List<ProductCategory>> GetAllAsync(IReadOnlyList<Guid> allowedPslIds, ProductServiceLineId? productServiceLineId = null, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<ProductCategory> Items, int TotalCount)> GetPagedAsync(ProductCategoryFilter filter, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default);
        Task AddAsync(ProductCategory category, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductCategory category, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameInPslAsync(string name, ProductServiceLineId pslId, ProductCategoryId? excludeId = null, CancellationToken cancellationToken = default);
    }
}
