using HB_ERP.SharedKernel.Domain.Common;

namespace Inventory.Domain.VO
{
    public readonly record struct ProductBrandId(Guid Value)
    {
        public static ProductBrandId New() => new ProductBrandId(Helper.GetNewCombSequentialID());
        public static ProductBrandId Create(Guid id) => new ProductBrandId(id);
    }
}
