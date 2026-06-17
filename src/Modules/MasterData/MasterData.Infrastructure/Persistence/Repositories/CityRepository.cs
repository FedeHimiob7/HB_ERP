using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class CityRepository : ICityRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public CityRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<City?> GetByIdAsync(CityId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Cities
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<List<City>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Cities
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameInStateAsync(string name, StateId stateId, CityId? excludeId = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Cities
                .AsNoTracking()
                .AnyAsync(c =>
                    c.Name.ToLower() == name.ToLower() &&
                    c.StateId == stateId &&
                    (excludeId == null || c.Id != excludeId.Value),
                    cancellationToken);
        }

        public async Task AddAsync(City city, CancellationToken cancellationToken = default)
        {
            await _dbContext.Cities.AddAsync(city, cancellationToken);
        }

        public Task UpdateAsync(City city, CancellationToken cancellationToken = default)
        {
            _dbContext.Cities.Update(city);
            return Task.CompletedTask;
        }

        public async Task<(IReadOnlyList<City> Cities, int TotalCount)> GetPagedAsync(
            CityFilter filter,
            CancellationToken cancellationToken = default)
        {
            IQueryable<City> query = _dbContext.Cities.AsNoTracking();

            if (filter.StateId.HasValue)
                query = query.Where(c => c.StateId == filter.StateId.Value);

            if (filter.CountryId.HasValue)
                query = query.Where(c => _dbContext.States.Any(s => s.Id == c.StateId && s.CountryId == filter.CountryId.Value));

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(term) ||
                    _dbContext.States.Any(s => s.Id == c.StateId && s.Name.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var cities = await query
                .OrderBy(c => c.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (cities, totalCount);
        }
    }
}
