using Inventory.Domain.Entities;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;

namespace Inventory.Domain.Repositories
{
    public interface IProductSubCategoryRepository
    {
        Task<ProductSubCategory?> GetByIdAsync(ProductSubCategoryId id, CancellationToken cancellationToken = default);
        Task<List<ProductSubCategory>> GetAllAsync(IReadOnlyList<Guid> allowedPslIds, ProductCategoryId? productCategoryId = null, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<ProductSubCategory> Items, int TotalCount)> GetPagedAsync(ProductSubCategoryFilter filter, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default);
        Task AddAsync(ProductSubCategory subCategory, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductSubCategory subCategory, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameInCategoryAsync(string name, ProductCategoryId categoryId, ProductSubCategoryId? excludeId = null, CancellationToken cancellationToken = default);
    }
}
