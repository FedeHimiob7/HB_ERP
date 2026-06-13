using Identity.Domain.Entities;
using Identity.Domain.Repositories;
using Identity.Domain.VO;
using Identity.Infrastructure.Persistence.Entities;
using Identity.Infrastructure.Persistence.Mappers;
using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly IdentityDbContext _db;

        public RoleRepository(IdentityDbContext dbContext)
        {
            _db = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Role?> GetByIdAsync(RoleId id, CancellationToken cancellationToken = default)
        {
            var entity = await _db.Set<RoleEntity>()
                .Include(r => r.RoleActions) 
                .FirstOrDefaultAsync(r => r.Id == id.Value, cancellationToken);

            return entity?.ToDomain();
        }
        public async Task AddAsync(Role role)
        {
            var entity = RoleMapper.ToEntity(role);
            await _db.Roles.AddAsync(entity);
        }

        public async Task UpdateAsync(Role role, CancellationToken cancellationToken = default)
        {
           
            var existingEntity = await _db.Set<RoleEntity>()
                .Include(r => r.RoleActions)
                .FirstOrDefaultAsync(r => r.Id == role.Id.Value, cancellationToken);

            if (existingEntity == null) return;

           
            existingEntity.Name = role.Name;
            existingEntity.IsActive = role.IsActive;

            var incomingActionIds = role.ActionIds.Select(a => a.Value).ToList();

            var actionsToRemove = existingEntity.RoleActions
                .Where(ra => !incomingActionIds.Contains(ra.ActionId))
                .ToList();

            foreach (var actionToRemove in actionsToRemove)
            {
                existingEntity.RoleActions.Remove(actionToRemove);
            }

            var existingActionIds = existingEntity.RoleActions.Select(ra => ra.ActionId).ToList();
            var newActionIds = incomingActionIds.Except(existingActionIds);

            foreach (var newActionId in newActionIds)
            {
                existingEntity.RoleActions.Add(new RoleActionEntity
                {
                    RoleId = existingEntity.Id,
                    ActionId = newActionId
                });
            }
        }

        public async Task<List<Role>> GetAllAsync()
        {
            var result = await _db.Set<RoleEntity>()
                .AsNoTracking()
                .ToListAsync();

            return result.Select(result => result.ToDomain()).ToList();
        }

        public async Task<List<RoleId>> GetExistingIdsAsync(IEnumerable<Guid> roleIds, CancellationToken cancellationToken = default)
        {
            
            var ids = roleIds.ToList();

            var existingIds = await _db.Set<RoleEntity>()
                .Where(r => ids.Contains(r.Id))
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            return existingIds.Select(RoleId.Create).ToList();
        }

        public async Task<List<Role>> GetRolesByActionIdAsync(Guid actionId, CancellationToken cancellationToken = default)
        {
            var entities = await _db.Set<RoleEntity>()
                .Include(r => r.RoleActions)
                .Where(r => r.RoleActions.Any(ra => ra.ActionId == actionId))
                .ToListAsync(cancellationToken);

            return entities.Select(RoleMapper.ToDomain).ToList();
        }

        public async Task<(IReadOnlyList<Role> roles, int totalCount)> GetPagedAsync(
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken)
        {
            var query = _db.Roles.AsNoTracking();

            int totalCount = await query.CountAsync(cancellationToken);

            var models = await query
                .Include(x => x.RoleActions)
                .OrderBy(u => u.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var domainRoles = models.Select(RoleMapper.ToDomain).ToList();

            return (domainRoles, totalCount);
        }
    }
}
