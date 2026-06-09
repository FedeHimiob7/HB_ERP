using HB_ERP.SharedKernel.Infrastructure;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Currencies.AsNoTracking();

            var totalCount = await query.CountAsync(cancellationToken);

            var currencies = await query
                .OrderBy(c => c.Code)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (currencies, totalCount);
        }
    }
}
