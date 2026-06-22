using HB_ERP.SharedKernel.Domain.Common;

namespace Inventory.Domain.VO
{
    public readonly record struct ProductSubCategoryId(Guid Value)
    {
        public static ProductSubCategoryId New() => new ProductSubCategoryId(Helper.GetNewCombSequentialID());
        public static ProductSubCategoryId Create(Guid id) => new ProductSubCategoryId(id);
    }
}
