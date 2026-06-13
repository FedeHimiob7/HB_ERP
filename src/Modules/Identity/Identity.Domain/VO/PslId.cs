using HB_ERP.SharedKernel.Domain.Common;

namespace Identity.Domain.VO
{
    public readonly record struct PslId(Guid Value)
    {
        public static PslId New()
        => new PslId(Helper.GetNewCombSequentialID());

        public static PslId Create(Guid id)
        => new PslId(id);
    }
}
