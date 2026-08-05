using HB_ERP.SharedKernel.Domain.Common;
using MasterData.Domain.VO;

namespace MasterData.Domain.SearchParametersModel
{
    public record FiscalTerminalFilter : PaginationFilter
    {
        public BranchId? BranchId { get; init; }

        public FiscalTerminalFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            BranchId? branchId = null)
            : base(pageNumber, pageSize, searchTerm)
        {
            BranchId = branchId;
        }
    }
}
