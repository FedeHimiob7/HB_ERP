using HB_ERP.SharedKernel.Domain.Common;

namespace MasterData.Domain.SearchParametersModel
{
    public record BranchFilter : PaginationFilter
    {
        public BranchFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null)
            : base(pageNumber, pageSize, searchTerm)
        {
        }
    }
}
