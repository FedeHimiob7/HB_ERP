using MasterData.Application.Interfaces;

namespace MasterData.Infrastructure.Persistence
{
    internal sealed class MasterDataEfUnitOfWork : IMasterDataUnitOfWork
    {
        private readonly MasterDataDbContext _dbContext;

        public MasterDataEfUnitOfWork(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public int GetHashCodeDeContexto() => _dbContext.GetHashCode();
    }
}
