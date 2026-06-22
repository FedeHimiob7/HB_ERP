using HB_ERP.SharedKernel.Domain.Common;

namespace Inventory.Domain.VO
{
    public readonly record struct ProductTypeId(Guid Value)
    {
        public static ProductTypeId New() => new ProductTypeId(Helper.GetNewCombSequentialID());
        public static ProductTypeId Create(Guid id) => new ProductTypeId(id);
    }
}
