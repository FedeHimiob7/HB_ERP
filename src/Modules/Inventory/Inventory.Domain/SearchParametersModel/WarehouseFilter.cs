using HB_ERP.SharedKernel.Domain.Common;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.SearchParametersModel
{
    public record WarehouseFilter : PaginationFilter
    {
        public ProductServiceLineId? ProductServiceLineId { get; init; }

        public WarehouseFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            ProductServiceLineId? productServiceLineId = null)
            : base(pageNumber, pageSize, searchTerm)
        {
            ProductServiceLineId = productServiceLineId;
        }
    }
}
