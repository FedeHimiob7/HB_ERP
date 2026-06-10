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
    internal sealed class StateRepository : IStateRepository
    {
        private readonly MasterDataDbContext _dbContext;

        public StateRepository(MasterDataDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<State?> GetByIdAsync(StateId id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.States
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<State?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _dbContext.States
                .FirstOrDefaultAsync(s => s.Code == code, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            return await _dbContext.States
                .AsNoTracking()
                .AnyAsync(s => s.Code == code, cancellationToken);
        }

        public async Task AddAsync(State state, CancellationToken cancellationToken = default)
        {
            await _dbContext.States.AddAsync(state, cancellationToken);
        }

        public Task UpdateAsync(State state, CancellationToken cancellationToken = default)
        {
            _dbContext.States.Update(state);
            return Task.CompletedTask;
        }

        public async Task<List<State>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.States
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<State> States, int TotalCount)> GetPagedAsync(
            StateFilter filter,
            CancellationToken cancellationToken = default)
        {
            IQueryable<State> query = _dbContext.States.AsNoTracking();

            // Filtros Dinámicos
            if (filter.CountryId.HasValue)
            {
                query = query.Where(s => s.CountryId == filter.CountryId.Value);
            }

            if (filter.IsActive.HasValue)
            {
                // IgnoreQueryFilters permite buscar inactivos si se solicita explícitamente, 
                // saltándose el HasQueryFilter de la configuración.
                query = query.IgnoreQueryFilters().Where(s => s.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();
                query = query.Where(s =>
                    s.Name.ToLower().Contains(term) ||
                    s.Code.ToLower().Contains(term) ||
                    _dbContext.Countries.Any(c => c.Id == s.CountryId && c.Name.ToLower().Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var states = await query
                .OrderBy(s => s.Name)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return (states, totalCount);
        }
    }
}
