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
    internal sealed class CountryRepository : ICountryRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public CountryRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Country?> GetByIdAsync(CountryId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Countries
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Countries
                .AsNoTracking()
                .AnyAsync(e => e.Name == name, cancellationToken);
        }

        public async Task AddAsync(Country country, CancellationToken cancellationToken = default)
        {
            await _dbContext.Countries.AddAsync(country, cancellationToken);
        }

        public Task UpdateAsync(Country country, CancellationToken cancellationToken = default)
        {
            _dbContext.Countries.Update(country);
            return Task.CompletedTask;
        }

        public async Task<List<Country>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Countries
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<Country> Countries, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.Countries.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(c => c.Name.Contains(searchTerm));

            var totalCount = await query.CountAsync(cancellationToken);

            var countries = await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (countries, totalCount);
        }
    }
}
