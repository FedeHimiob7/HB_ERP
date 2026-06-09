using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Models
{
    public record StateResponse(
    Guid Id,
    Guid CountryId,
    string Code,
    string Name);
}
