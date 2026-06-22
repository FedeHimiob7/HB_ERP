using Inventory.Domain.Entities;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;

namespace Inventory.Domain.Repositories
{
    public interface IProductBrandRepository
    {
        Task<ProductBrand?> GetByIdAsync(ProductBrandId id, CancellationToken cancellationToken = default);
        Task<List<ProductBrand>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<ProductBrand> Items, int TotalCount)> GetPagedAsync(ProductBrandFilter filter, CancellationToken cancellationToken = default);
        Task AddAsync(ProductBrand brand, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductBrand brand, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, ProductBrandId? excludeId = null, CancellationToken cancellationToken = default);
    }
}
