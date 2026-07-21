using HB_ERP.SharedKernel.Domain.Common;
using Inventory.Domain.VO;

namespace Inventory.Domain.SearchParametersModel
{
    public record ProductFilter : PaginationFilter
    {
        public Guid? ProductServiceLineId { get; init; }
        public Guid? ProductTypeId { get; init; }
        public Guid? ProductCategoryId { get; init; }
        public Guid? ProductBrandId { get; init; }
        public bool? IsSalable { get; init; }
        public bool? IsPurchasable { get; init; }

        public ProductFilter(
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null,
            Guid? productServiceLineId = null,
            Guid? productTypeId = null,
            Guid? productCategoryId = null,
            Guid? productBrandId = null,
            bool? isSalable = null,
            bool? isPurchasable = null)
            : base(pageNumber, pageSize, searchTerm)
        {
            ProductServiceLineId = productServiceLineId;
            ProductTypeId = productTypeId;
            ProductCategoryId = productCategoryId;
            ProductBrandId = productBrandId;
            IsSalable = isSalable;
            IsPurchasable = isPurchasable;
        }
    }
}
