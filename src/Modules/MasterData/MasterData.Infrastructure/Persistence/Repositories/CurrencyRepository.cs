using HB_ERP.SharedKernel.Infrastructure;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class CurrencyRepository : ICurrencyRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public CurrencyRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Currency?> GetByIdAsync(CurrencyId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Currencies
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<Currency?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Currencies
                .FirstOrDefaultAsync(e => e.Code == code, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Currencies
                .AsNoTracking() 
                .AnyAsync(e => e.Code == code, cancellationToken);
        }

        public async Task AddAsync(Currency currency, CancellationToken cancellationToken = default)
        {
            await _dbContext.Currencies.AddAsync(currency, cancellationToken);
        }

        public Task UpdateAsync(Currency currency, CancellationToken cancellationToken = default)
        {
            _dbContext.Currencies.Update(currency);
            return Task.CompletedTask;
        }

        public async Task<List<Currency>> GetAllAsync(CancellationToken cancellationToken = default)
        {           
            return await _dbContext.Currencies
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Currency> Currencies, int TotalCount)> GetPagedAsync(
            CurrencyFilter filter,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Currencies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(c =>
                    c.Code.ToLower().Contains(term) ||
                    c.Name.ToLower().Contains(term) ||
                    c.Symbol.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var currencies = await query
                .OrderBy(c => c.Code)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (currencies, totalCount);
        }
    }
}
