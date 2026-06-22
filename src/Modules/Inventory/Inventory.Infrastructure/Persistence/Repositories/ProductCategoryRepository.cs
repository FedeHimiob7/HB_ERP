using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductCategoryRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductCategory?> GetByIdAsync(ProductCategoryId id, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default)
        {
            var pslVoList = allowedPslIds.Select(g => new ProductServiceLineId(g)).ToList();
            return await _dbContext.ProductCategories
                .FirstOrDefaultAsync(x => x.Id == id && pslVoList.Contains(x.ProductServiceLineId), cancellationToken);
        }

        public async Task<List<ProductCategory>> GetAllAsync(IReadOnlyList<Guid> allowedPslIds, ProductServiceLineId? productServiceLineId = null, CancellationToken cancellationToken = default)
        {
            var pslVoList = allowedPslIds.Select(g => new ProductServiceLineId(g)).ToList();
            var query = _dbContext.ProductCategories.AsNoTracking()
                .Where(x => pslVoList.Contains(x.ProductServiceLineId));

            if (productServiceLineId.HasValue)
                query = query.Where(x => x.ProductServiceLineId == productServiceLineId.Value);

            return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<ProductCategory> Items, int TotalCount)> GetPagedAsync(
            ProductCategoryFilter filter, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default)
        {
            var pslVoList = allowedPslIds.Select(g => new ProductServiceLineId(g)).ToList();
            var query = _dbContext.ProductCategories.AsNoTracking()
                .Where(x => pslVoList.Contains(x.ProductServiceLineId));

            if (filter.ProductServiceLineId.HasValue)
                query = query.Where(x => x.ProductServiceLineId == filter.ProductServiceLineId.Value);

            if (filter.ProductTypeId.HasValue)
                query = query.Where(x => x.ProductTypeId == filter.ProductTypeId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task AddAsync(ProductCategory productCategory, CancellationToken cancellationToken = default)
            => await _dbContext.ProductCategories.AddAsync(productCategory, cancellationToken);

        public Task UpdateAsync(ProductCategory productCategory, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductCategories.Update(productCategory);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByNameInPslAsync(string name, ProductServiceLineId pslId, ProductCategoryId? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductCategories.AsNoTracking()
                .Where(x => x.Name.ToLower() == name.ToLower() && x.ProductServiceLineId == pslId);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return await query.AnyAsync(cancellationToken);
        }
    }
}
