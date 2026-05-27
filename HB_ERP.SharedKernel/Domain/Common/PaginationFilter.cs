using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.Domain.Common
{
    public abstract record PaginationFilter(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null);
}
