using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Models
{  
    public record ProductServiceLineResponse(
    Guid Id,
    string description,
    string Name);
}
