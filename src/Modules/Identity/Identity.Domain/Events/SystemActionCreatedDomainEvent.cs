using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity.Domain.VO;

namespace Identity.Domain.Events
{
    public sealed record SystemActionCreatedDomainEvent(
     Guid Id,
     Guid ActionId,
     string Name) : DomainEvent(Id);
}
