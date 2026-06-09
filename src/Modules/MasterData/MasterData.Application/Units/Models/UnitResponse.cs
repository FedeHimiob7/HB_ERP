using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Models
{
    public record UnitResponse(Guid Id, string Name, string Description, bool IsActive);
}
