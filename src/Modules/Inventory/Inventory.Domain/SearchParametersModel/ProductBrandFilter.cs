using HB_ERP.SharedKernel.Domain.Common;

namespace Inventory.Domain.SearchParametersModel
{
    public record ProductBrandFilter(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null) : PaginationFilter(PageNumber, PageSize, SearchTerm);
}
