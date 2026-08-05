using ErrorOr;
using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class BranchRepository : IBranchRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public BranchRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Branch?> GetByIdAsync(BranchId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Branches
                .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Branches
                .AsNoTracking()
                .OrderBy(b => b.SequenceNumber)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Branch> Branches, int TotalCount)> GetPagedAsync(
            BranchFilter filter,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Branch> query = _dbContext.Branches.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(b =>
                    b.Name.ToLower().Contains(term) ||
                    b.Address.ToLower().Contains(term));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var branches = await query
                .OrderBy(b => b.SequenceNumber)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (branches, totalCount);
        }

        public async Task<ErrorOr<Branch>> ReserveNextSequenceNumberAndAddAsync(
            Func<int, ErrorOr<Branch>> factory,
            CancellationToken cancellationToken = default)
        {
            await using var tx = await _dbContext.Database.BeginTransactionAsync(
                System.Data.IsolationLevel.Serializable, cancellationToken);

            try
            {
                // UPDLOCK + HOLDLOCK garantiza que dos requests simultáneos no lean el mismo valor
                await _dbContext.Branches
                    .FromSqlRaw("SELECT * FROM [MasterData].[Branches] WITH (UPDLOCK, HOLDLOCK, ROWLOCK)")
                    .IgnoreQueryFilters()
                    .ToListAsync(cancellationToken);

                var maxSequenceNumber = await _dbContext.Branches
                    .IgnoreQueryFilters()
                    .MaxAsync(b => (int?)b.SequenceNumber, cancellationToken) ?? 0;

                var createResult = factory(maxSequenceNumber + 1);
                if (createResult.IsError)
                {
                    await tx.RollbackAsync(cancellationToken);
                    return createResult.Errors;
                }

                var branch = createResult.Value;
                _dbContext.Branches.Add(branch);

                await _dbContext.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);

                return branch;
            }
            catch
            {
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public Task UpdateAsync(Branch branch, CancellationToken cancellationToken = default)
        {
            _dbContext.Branches.Update(branch);
            return Task.CompletedTask;
        }
    }
}
