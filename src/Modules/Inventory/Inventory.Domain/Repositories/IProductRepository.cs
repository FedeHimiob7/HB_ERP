using Inventory.Domain.Entities;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(ProductId id, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default);
        Task<List<Product>> GetAllAsync(IReadOnlyList<Guid> allowedPslIds, ProductServiceLineId? productServiceLineId = null, CancellationToken cancellationToken = default);
        Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedAsync(ProductFilter filter, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string code, ProductId? excludeId = null, CancellationToken cancellationToken = default);
        Task<ProductPriceHistory?> GetLastPriceHistoryAsync(ProductId id, CancellationToken cancellationToken = default);
    }
}
