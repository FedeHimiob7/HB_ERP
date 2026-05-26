using HB_ERP.SharedKernel.Domain.Common;

namespace Identity.Domain.VO
{
    public readonly record struct ActionsId(Guid Value)
    {
        public static ActionsId New()
        => new ActionsId(Helper.GetNewCombSequentialID());

        public static ActionsId Create(Guid id)
        => new ActionsId(id);
    }
}
