using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductSubCategoryRepository : IProductSubCategoryRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductSubCategoryRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductSubCategory?> GetByIdAsync(ProductSubCategoryId id, CancellationToken cancellationToken = default)
            => await _dbContext.ProductSubCategories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<List<ProductSubCategory>> GetAllAsync(IReadOnlyList<Guid> allowedPslIds, ProductCategoryId? productCategoryId = null, CancellationToken cancellationToken = default)
        {
            var pslVoList = allowedPslIds.Select(g => new ProductServiceLineId(g)).ToList();
            var query = _dbContext.ProductSubCategories.AsNoTracking()
                .Where(x => _dbContext.ProductCategories
                    .Any(c => c.Id == x.ProductCategoryId && pslVoList.Contains(c.ProductServiceLineId)));

            if (productCategoryId.HasValue)
                query = query.Where(x => x.ProductCategoryId == productCategoryId.Value);

            return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<ProductSubCategory> Items, int TotalCount)> GetPagedAsync(
            ProductSubCategoryFilter filter, IReadOnlyList<Guid> allowedPslIds, CancellationToken cancellationToken = default)
        {
            var pslVoList = allowedPslIds.Select(g => new ProductServiceLineId(g)).ToList();
            var query = _dbContext.ProductSubCategories.AsNoTracking()
                .Where(x => _dbContext.ProductCategories
                    .Any(c => c.Id == x.ProductCategoryId && pslVoList.Contains(c.ProductServiceLineId)));

            if (filter.ProductCategoryId.HasValue)
                query = query.Where(x => x.ProductCategoryId == filter.ProductCategoryId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(x =>
                    x.Name.ToLower().Contains(term) ||
                    _dbContext.ProductCategories.Any(c => c.Id == x.ProductCategoryId && c.Name.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task AddAsync(ProductSubCategory productSubCategory, CancellationToken cancellationToken = default)
            => await _dbContext.ProductSubCategories.AddAsync(productSubCategory, cancellationToken);

        public Task UpdateAsync(ProductSubCategory productSubCategory, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductSubCategories.Update(productSubCategory);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByNameInCategoryAsync(string name, ProductCategoryId categoryId, ProductSubCategoryId? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductSubCategories.AsNoTracking()
                .Where(x => x.Name.ToLower() == name.ToLower() && x.ProductCategoryId == categoryId);
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return await query.AnyAsync(cancellationToken);
        }
    }
}
