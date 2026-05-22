using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain
{
    public abstract record DomainEvent(Guid Id) : INotification
    {       
        public DateTime OccurredOnUtc { get; protected set; } = DateTime.UtcNow;
    }
}
