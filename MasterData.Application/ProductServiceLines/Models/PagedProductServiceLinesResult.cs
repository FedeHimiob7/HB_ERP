using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.ProductServiceLines.Models
{
    public record PagedProductServiceLinesResult(
    IReadOnlyList<ProductServiceLineResponse> Items,
    int TotalCount);
}
