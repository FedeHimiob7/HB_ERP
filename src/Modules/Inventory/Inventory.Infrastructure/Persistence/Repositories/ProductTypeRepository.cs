using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductTypeRepository : IProductTypeRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductTypeRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductType?> GetByIdAsync(ProductTypeId id, CancellationToken cancellationToken = default)
            => await _dbContext.ProductTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<List<ProductType>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbContext.ProductTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

        public async Task<(IReadOnlyList<ProductType> Items, int TotalCount)> GetPagedAsync(
            ProductTypeFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductTypes.AsNoTracking();

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

        public async Task AddAsync(ProductType productType, CancellationToken cancellationToken = default)
            => await _dbContext.ProductTypes.AddAsync(productType, cancellationToken);

        public Task UpdateAsync(ProductType productType, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductTypes.Update(productType);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByNameAsync(string name, ProductTypeId? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductTypes.AsNoTracking().Where(x => x.Name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return await query.AnyAsync(cancellationToken);
        }
    }
}
