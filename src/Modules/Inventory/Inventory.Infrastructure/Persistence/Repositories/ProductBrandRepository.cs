using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    internal sealed class ProductBrandRepository : IProductBrandRepository
    {
        private readonly InventoryDbContext _dbContext;

        public ProductBrandRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ProductBrand?> GetByIdAsync(ProductBrandId id, CancellationToken cancellationToken = default)
            => await _dbContext.ProductBrands.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<List<ProductBrand>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbContext.ProductBrands.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

        public async Task<(IReadOnlyList<ProductBrand> Items, int TotalCount)> GetPagedAsync(
            ProductBrandFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductBrands.AsNoTracking();

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

        public async Task AddAsync(ProductBrand productBrand, CancellationToken cancellationToken = default)
            => await _dbContext.ProductBrands.AddAsync(productBrand, cancellationToken);

        public Task UpdateAsync(ProductBrand productBrand, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductBrands.Update(productBrand);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByNameAsync(string name, ProductBrandId? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProductBrands.AsNoTracking().Where(x => x.Name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return await query.AnyAsync(cancellationToken);
        }
    }
}
