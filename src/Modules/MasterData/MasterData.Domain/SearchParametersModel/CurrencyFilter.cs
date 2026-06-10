using HB_ERP.SharedKernel.Domain.Common;

namespace MasterData.Domain.SearchParametersModel
{
    public record CurrencyFilter : PaginationFilter
    {
        public CurrencyFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null)
            : base(pageNumber, pageSize, searchTerm)
        {
        }
    }
}
