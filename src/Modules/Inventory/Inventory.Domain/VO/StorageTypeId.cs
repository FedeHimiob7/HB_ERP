using HB_ERP.SharedKernel.Domain.Common;

namespace Inventory.Domain.VO
{
    public readonly record struct StorageTypeId(Guid Value)
    {
        public static StorageTypeId New() => new StorageTypeId(Helper.GetNewCombSequentialID());
        public static StorageTypeId Create(Guid id) => new StorageTypeId(id);
    }
}
