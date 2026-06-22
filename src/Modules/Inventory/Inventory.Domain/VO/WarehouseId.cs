using HB_ERP.SharedKernel.Domain.Common;

namespace Inventory.Domain.VO
{
    public readonly record struct WarehouseId(Guid Value)
    {
        public static WarehouseId New() => new WarehouseId(Helper.GetNewCombSequentialID());
        public static WarehouseId Create(Guid id) => new WarehouseId(id);
    }
}
