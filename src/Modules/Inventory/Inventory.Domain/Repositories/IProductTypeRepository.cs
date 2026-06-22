using Inventory.Domain.Entities;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;

namespace Inventory.Domain.Repositories
{
    public interface IProductTypeRepository
    {
        Task<ProductType?> GetByIdAsync(ProductTypeId id, CancellationToken cancellationToken = default);
        Task<List<ProductType>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<ProductType> Items, int TotalCount)> GetPagedAsync(ProductTypeFilter filter, CancellationToken cancellationToken = default);
        Task AddAsync(ProductType productType, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductType productType, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, ProductTypeId? excludeId = null, CancellationToken cancellationToken = default);
    }
}
