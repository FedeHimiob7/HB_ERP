using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class CompanyRepository : ICompanyRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public CompanyRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Company?> GetSingletonAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Companies
                .FirstOrDefaultAsync(c => c.Id == CompanyId.Singleton, cancellationToken);
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Companies
                .AsNoTracking()
                .AnyAsync(c => c.Id == CompanyId.Singleton, cancellationToken);
        }

        public async Task AddAsync(Company company, CancellationToken cancellationToken = default)
        {
            await _dbContext.Companies.AddAsync(company, cancellationToken);
        }

        public Task UpdateAsync(Company company, CancellationToken cancellationToken = default)
        {
            _dbContext.Companies.Update(company);
            return Task.CompletedTask;
        }
    }
}
