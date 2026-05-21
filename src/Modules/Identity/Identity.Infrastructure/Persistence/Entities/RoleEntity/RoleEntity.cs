using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Entities
{
    public class RoleEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
        public ICollection<RoleActionEntity> RoleActions { get; set; } = new List<RoleActionEntity>();
    }
}
