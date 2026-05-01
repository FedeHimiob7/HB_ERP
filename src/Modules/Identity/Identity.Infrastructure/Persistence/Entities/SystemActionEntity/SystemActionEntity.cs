using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence.Entities.SystemActionEntity
{
    public class SystemActionEntity
    {
            public Guid Id { get; set; }
            public string Name { get; set; } = default!;
            public string? Description { get; set; }
            public bool IsActive { get; set; }
    }
}
