using HB_ERP.SharedKernel.Infrastructure;

namespace Inventory.Infrastructure.Persistence.Repositories
{
    internal sealed class OutboxRepository : IOutboxRepository
    {
        private readonly InventoryDbContext _dbContext;

        public OutboxRepository(InventoryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default)
        {
            await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
        }
    }
}
