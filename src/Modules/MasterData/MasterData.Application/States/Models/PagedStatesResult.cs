using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Models
{
    public record PagedStatesResult(IReadOnlyList<StateResponse> Items, int TotalCount);
}
