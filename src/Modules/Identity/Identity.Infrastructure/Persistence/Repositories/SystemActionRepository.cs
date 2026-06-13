using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using Identity.Infrastructure.Persistence.Entities;
using Identity.Infrastructure.Persistence.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories
{
    internal sealed class SystemActionRepository : ISystemActionRepository
    {
        private readonly IdentityDbContext _db;

        public SystemActionRepository(IdentityDbContext dbContext)
        {
            _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<SystemAction?> GetByIdAsync(ActionsId id)
        {
            var entity = await _db.Set<SystemActionEntity>()
                .FirstOrDefaultAsync(e => e.Id == id.Value);

            return entity?.ToDomain();
        }

        public async Task<List<SystemAction>> GetAllAsync()
        {
            var entities = await _db.Set<SystemActionEntity>().ToListAsync();
            return entities.Select(e => e.ToDomain()).ToList();
        }

        public async Task<List<ActionsId>> GetExistingIdsAsync(IEnumerable<Guid> actionIds, CancellationToken cancellationToken = default)
        {
            var ids = actionIds.ToList();
            var existingIds = await _db.Set<SystemActionEntity>()
                .Where(a => ids.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync(cancellationToken);

            return existingIds.Select(id => new ActionsId(id)).ToList();
        }

        public async Task<bool> IsNameUniqueAsync(string name, CancellationToken cancellationToken = default)
        {
            return !await _db.Set<SystemActionEntity>()
                .AnyAsync(e => e.Name == name, cancellationToken);
        }

        public async Task AddAsync(SystemAction systemAction, CancellationToken cancellationToken = default)
        {
            var entity = systemAction.ToEntity();
            await _db.Set<SystemActionEntity>().AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(SystemAction action)
        {
            var existingEntity = await _db.Set<SystemActionEntity>()
                .FirstOrDefaultAsync(e => e.Id == action.Id.Value);

            if (existingEntity is null) return;

            existingEntity.Name = action.Name.Value;
            existingEntity.Description = action.Description;
            existingEntity.IsActive = action.IsActive;
        }

        public async Task<(IReadOnlyList<SystemAction> Actions, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _db.Set<SystemActionEntity>().AsNoTracking();

            int totalCount = await query.CountAsync(cancellationToken);

            var entities = await query
                .OrderBy(a => a.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (entities.Select(e => e.ToDomain()).ToList(), totalCount);
        }
    }
}
