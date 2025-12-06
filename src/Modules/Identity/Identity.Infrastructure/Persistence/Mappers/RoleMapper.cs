using Identity.Domain.Entities;
using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Mappers
{
    public static class RoleMapper
    {
        public static Role ToDomain(RoleEntity entity)
        {
            return Role.CreateExisting(
                id: new RoleId(entity.Id),
                name: entity.Name
            );
        }

        public static RoleEntity ToEntity(Role role)
        {
            return new RoleEntity
            {
                Id = role.Id.Value,
                Name = role.Name
            };
        }
    }
}
