using HB_ERP.SharedKernel.Domain.Common;
using MasterData.Domain.Enums;

namespace MasterData.Domain.SearchParametersModel
{
    public record TaxFilter : PaginationFilter
    {
        public TaxType? TaxType { get; init; }

        public TaxFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            TaxType? taxType = null)
            : base(pageNumber, pageSize, searchTerm)
        {
            TaxType = taxType;
        }
    }
}
