using HB_ERP.SharedKernel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.SearchParametersModel
{
    public record UnitFilter : PaginationFilter
    {
        public UnitFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null)
            : base(pageNumber, pageSize, searchTerm)
        {
        }
    }
}
