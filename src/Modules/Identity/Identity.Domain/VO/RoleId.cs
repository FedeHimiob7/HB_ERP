using HB_ERP.SharedKernel.Domain.Common;

namespace Identity.Domain.VO
{
    public readonly record struct RoleId(Guid Value)
    {
        public static RoleId New()
        => new RoleId(Helper.GetNewCombSequentialID());

        public static RoleId Create(Guid id)
        => new RoleId(id);
    }
}
