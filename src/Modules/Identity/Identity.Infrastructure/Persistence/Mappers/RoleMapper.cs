using Identity.Domain.Entities;
using Identity.Domain.VO;
using Identity.Infrastructure.Persistence.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Mappers
{
    public static class RoleMapper
    {
        public static RoleEntity ToEntity(this Role domain)
        {
            return new RoleEntity
            {
                Id = domain.Id.Value,
                Name = domain.Name,
                RoleActions = domain.ActionIds.Select(actionId => new RoleActionEntity
                {
                    RoleId = domain.Id.Value,
                    ActionId = actionId.Value
                }).ToList()
            };
        }

        public static Role ToDomain(this RoleEntity entity)
        {
            var role = Role.CreateExisting(
                new RoleId(entity.Id),
                entity.Name
            );

            if (entity.RoleActions != null && entity.RoleActions.Any())
            {
                foreach (var roleAction in entity.RoleActions)
                {
                    role.AssignAction(new ActionsId(roleAction.ActionId));
                }

                role.ClearDomainEvents();
            }

            return role;
        }
    }
}
