using Inventory.Application.Interfaces;

namespace Inventory.Infrastructure.Persistence
{
    internal sealed class InventoryEfUnitOfWork : IInventoryUnitOfWork
    {
        private readonly InventoryDbContext _dbContext;

        public InventoryEfUnitOfWork(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
