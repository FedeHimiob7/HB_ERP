using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Entities
{
    public class RoleActionEntity
    {
        public Guid RoleId { get; set; }
        public Guid ActionId { get; set; }
        public RoleEntity Role { get; set; } = null!;
        public SystemActionEntity SystemAction { get; set; } = null!;
    }
}
