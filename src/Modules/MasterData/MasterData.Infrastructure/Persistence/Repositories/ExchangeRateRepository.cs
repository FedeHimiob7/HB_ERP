using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class ExchangeRateRepository : IExchangeRateRepository
    {
        private readonly MasterDataDbContext _dbContext;
        public ExchangeRateRepository(MasterDataDbContext dbContext) => _dbContext = dbContext;

        public async Task<ExchangeRate?> GetByIdAsync(ExchangeRateId id, CancellationToken cancellationToken = default)
            => await _dbContext.ExchangeRates.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        public async Task<ExchangeRate?> GetLatestAsync(CancellationToken cancellationToken = default)
            => await _dbContext.ExchangeRates.AsNoTracking()
                .OrderByDescending(e => e.RegisterDate)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<ExchangeRate?> GetLatestByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
        {
            var upperBound = date.AddDays(1).ToDateTime(TimeOnly.MinValue);
            return await _dbContext.ExchangeRates.AsNoTracking()
                .Where(e => e.RegisterDate < upperBound)
                .OrderByDescending(e => e.RegisterDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<ExchangeRate> Items, int TotalCount)> GetPagedAsync(
            int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ExchangeRates.AsNoTracking()
                .OrderByDescending(e => e.RegisterDate);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public async Task AddAsync(ExchangeRate exchangeRate, CancellationToken cancellationToken = default)
            => await _dbContext.ExchangeRates.AddAsync(exchangeRate, cancellationToken);
    }
}
