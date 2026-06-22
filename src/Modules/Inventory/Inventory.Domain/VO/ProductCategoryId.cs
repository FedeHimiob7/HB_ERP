using HB_ERP.SharedKernel.Domain.Common;

namespace Inventory.Domain.VO
{
    public readonly record struct ProductCategoryId(Guid Value)
    {
        public static ProductCategoryId New() => new ProductCategoryId(Helper.GetNewCombSequentialID());
        public static ProductCategoryId Create(Guid id) => new ProductCategoryId(id);
    }
}
