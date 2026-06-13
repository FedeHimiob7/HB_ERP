using HB_ERP.SharedKernel.Domain;
using Identity.Domain.VO;

namespace Identity.Domain.Events
{
    public sealed record UserPslAssignedDomainEvent(
        Guid Id,
        UserId UserId,
        PslId PslId
    ) : DomainEvent(Id);
}
