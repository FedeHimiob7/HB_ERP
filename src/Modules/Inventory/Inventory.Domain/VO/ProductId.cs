using HB_ERP.SharedKernel.Domain.Common;

namespace Inventory.Domain.VO
{
    public readonly record struct ProductId(Guid Value)
    {
        public static ProductId New() => new ProductId(Helper.GetNewCombSequentialID());
        public static ProductId Create(Guid id) => new ProductId(id);
    }
}
