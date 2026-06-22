using HB_ERP.SharedKernel.Domain.Common;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.SearchParametersModel
{
    public record ProductCategoryFilter : PaginationFilter
    {
        public ProductServiceLineId? ProductServiceLineId { get; init; }
        public ProductTypeId? ProductTypeId { get; init; }

        public ProductCategoryFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            ProductServiceLineId? productServiceLineId = null,
            ProductTypeId? productTypeId = null)
            : base(pageNumber, pageSize, searchTerm)
        {
            ProductServiceLineId = productServiceLineId;
            ProductTypeId = productTypeId;
        }
    }
}
