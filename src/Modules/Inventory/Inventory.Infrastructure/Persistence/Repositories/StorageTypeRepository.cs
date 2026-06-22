using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.SearchParametersModel;
using Inventory.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    internal sealed class StorageTypeRepository : IStorageTypeRepository
    {
        private readonly InventoryDbContext _dbContext;

        public StorageTypeRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StorageType?> GetByIdAsync(StorageTypeId id, CancellationToken cancellationToken = default)
            => await _dbContext.StorageTypes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public async Task<List<StorageType>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbContext.StorageTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync(cancellationToken);

        public async Task<(IReadOnlyList<StorageType> Items, int TotalCount)> GetPagedAsync(
            StorageTypeFilter filter, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.StorageTypes.AsNoTracking();

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

        public async Task AddAsync(StorageType storageType, CancellationToken cancellationToken = default)
            => await _dbContext.StorageTypes.AddAsync(storageType, cancellationToken);

        public Task UpdateAsync(StorageType storageType, CancellationToken cancellationToken = default)
        {
            _dbContext.StorageTypes.Update(storageType);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByNameAsync(string name, StorageTypeId? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.StorageTypes.AsNoTracking().Where(x => x.Name.ToLower() == name.ToLower());
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            return await query.AnyAsync(cancellationToken);
        }
    }
}
