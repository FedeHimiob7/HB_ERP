using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class FiscalTerminalRepository : IFiscalTerminalRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public FiscalTerminalRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<FiscalTerminal?> GetByIdAsync(FiscalTerminalId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.FiscalTerminals
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<List<FiscalTerminal>> GetAllAsync(BranchId? branchId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.FiscalTerminals.AsNoTracking();

            if (branchId.HasValue)
                query = query.Where(f => f.BranchId == branchId.Value);

            return await query.OrderBy(f => f.Name).ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<FiscalTerminal> FiscalTerminals, int TotalCount)> GetPagedAsync(
            FiscalTerminalFilter filter,
            CancellationToken cancellationToken = default)
        {
            IQueryable<FiscalTerminal> query = _dbContext.FiscalTerminals.AsNoTracking();

            if (filter.BranchId.HasValue)
            {
                query = query.Where(f => f.BranchId == filter.BranchId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(f => f.Name.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var fiscalTerminals = await query
                .OrderBy(f => f.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (fiscalTerminals, totalCount);
        }

        public async Task AddAsync(FiscalTerminal fiscalTerminal, CancellationToken cancellationToken = default)
        {
            await _dbContext.FiscalTerminals.AddAsync(fiscalTerminal, cancellationToken);
        }

        public Task UpdateAsync(FiscalTerminal fiscalTerminal, CancellationToken cancellationToken = default)
        {
            _dbContext.FiscalTerminals.Update(fiscalTerminal);
            return Task.CompletedTask;
        }
    }
}
