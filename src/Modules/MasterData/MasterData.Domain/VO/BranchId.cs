using HB_ERP.SharedKernel.Domain.Common;
using System;

namespace MasterData.Domain.VO
{
    public readonly record struct BranchId(Guid Value)
    {
        public static BranchId New()
            => new BranchId(Helper.GetNewCombSequentialID());

        public static BranchId Create(Guid id)
            => new BranchId(id);
    }
}
