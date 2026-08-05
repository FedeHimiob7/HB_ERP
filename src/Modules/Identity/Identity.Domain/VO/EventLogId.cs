using HB_ERP.SharedKernel.Domain.Common;

namespace Identity.Domain.VO
{
    public readonly record struct EventLogId(Guid Value)
    {
        public static EventLogId New()
        => new EventLogId(Helper.GetNewCombSequentialID());

        public static EventLogId Create(Guid id)
        => new EventLogId(id);
    }
}
