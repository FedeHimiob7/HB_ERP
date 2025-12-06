using Identity.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Events
{
    public sealed record UserDeactivatedDomainEvent(
    Guid Id,
    UserId UserId) : DomainEvent(Id);
}
