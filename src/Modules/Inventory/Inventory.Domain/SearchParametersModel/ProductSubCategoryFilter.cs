using HB_ERP.SharedKernel.Domain.Common;
using Inventory.Domain.VO;

namespace Inventory.Domain.SearchParametersModel
{
    public record ProductSubCategoryFilter : PaginationFilter
    {
        public ProductCategoryId? ProductCategoryId { get; init; }

        public ProductSubCategoryFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            ProductCategoryId? productCategoryId = null)
            : base(pageNumber, pageSize, searchTerm)
        {
            ProductCategoryId = productCategoryId;
        }
    }
}
