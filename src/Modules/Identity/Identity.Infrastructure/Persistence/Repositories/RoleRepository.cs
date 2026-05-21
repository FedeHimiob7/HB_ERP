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
            // 1. Traemos la entidad de la BD con Tracking habilitado
            var existingEntity = await _db.Set<RoleEntity>()
                .Include(r => r.RoleActions)
                .FirstOrDefaultAsync(r => r.Id == role.Id.Value, cancellationToken);

            if (existingEntity == null) return;

            // 2. Actualizamos propiedades planas
            existingEntity.Name = role.Name;

            // 3. Sincronizar la colección de acciones
            var incomingActionIds = role.ActionIds.Select(a => a.Value).ToList();

            // A. Eliminar las que ya no están en el dominio
            var actionsToRemove = existingEntity.RoleActions
                .Where(ra => !incomingActionIds.Contains(ra.ActionId))
                .ToList();

            foreach (var actionToRemove in actionsToRemove)
            {
                existingEntity.RoleActions.Remove(actionToRemove);
            }

            // B. Agregar las nuevas que vienen del dominio
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

        public Task<List<Role>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
