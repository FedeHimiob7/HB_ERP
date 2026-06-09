using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Common.Models
{
    public record RoleResponse(
        Guid Id,
        string Name,
        List<Guid> Actions);
}
