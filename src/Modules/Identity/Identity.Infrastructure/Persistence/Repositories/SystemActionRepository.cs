using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using Identity.Infrastructure.Persistence.Entities.SystemActionEntity;
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

        public Task UpdateAsync(SystemAction action)
        {
            var entity = action.ToEntity();
            _db.Set<SystemActionEntity>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
