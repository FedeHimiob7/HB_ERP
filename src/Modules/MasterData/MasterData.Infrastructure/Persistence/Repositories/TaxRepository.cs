using MasterData.Domain.Entities;
using MasterData.Domain.Enums;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class TaxRepository : ITaxRepository
    {
        private readonly MasterDataDbContext _dbContext;
        public TaxRepository(MasterDataDbContext dbContext) => _dbContext = dbContext;

        public async Task<Tax?> GetByIdAsync(TaxId id, CancellationToken cancellationToken = default)
            => await _dbContext.Taxes.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        public async Task<List<Tax>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbContext.Taxes.AsNoTracking()
                .OrderBy(t => t.TaxType).ThenBy(t => t.Name)
                .ToListAsync(cancellationToken);

        public async Task AddAsync(Tax tax, CancellationToken cancellationToken = default)
            => await _dbContext.Taxes.AddAsync(tax, cancellationToken);

        public Task UpdateAsync(Tax tax, CancellationToken cancellationToken = default)
        {
            _dbContext.Taxes.Update(tax);
            return Task.CompletedTask;
        }

        public async Task<(IReadOnlyList<Tax> Taxes, int TotalCount)> GetPagedAsync(
            TaxFilter filter,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Tax> query = _dbContext.Taxes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(t => t.Name.ToLower().Contains(term));
            }

            if (filter.TaxType.HasValue)
                query = query.Where(t => t.TaxType == filter.TaxType.Value);

            var totalCount = await query.CountAsync(cancellationToken);

            var taxes = await query
                .OrderBy(t => t.TaxType)
                .ThenBy(t => t.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (taxes, totalCount);
        }
    }
}
