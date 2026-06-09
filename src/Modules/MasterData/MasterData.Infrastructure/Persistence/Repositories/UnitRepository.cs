using MasterData.Domain.Entities;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MasterData.Domain.VO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence.Repositories
{
    internal sealed class UnitRepository : IUnitRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public UnitRepository(MasterDataDbContext dbContext) => _dbContext = dbContext;

        public async Task<Unit?> GetByIdAsync(UnitId id, CancellationToken cancellationToken = default)
            => await _dbContext.Units.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

        public async Task AddAsync(Unit unit, CancellationToken cancellationToken = default)
            => await _dbContext.Units.AddAsync(unit, cancellationToken);

        public Task UpdateAsync(Unit unit, CancellationToken cancellationToken = default)
        {
            _dbContext.Units.Update(unit);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
            => await _dbContext.Units.AsNoTracking().AnyAsync(u => u.Name == name, cancellationToken);

        public async Task<List<Unit>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbContext.Units.AsNoTracking().OrderBy(u => u.Name).ToListAsync(cancellationToken);

        public async Task<(IReadOnlyList<Unit> Units, int TotalCount)> GetPagedAsync(
            UnitFilter filter,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Unit> query = _dbContext.Units.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(u => u.Name.ToLower().Contains(searchTerm) ||
                                         u.Description.ToLower().Contains(searchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var units = await query
                .OrderBy(u => u.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (units, totalCount);
        }
    }
}
