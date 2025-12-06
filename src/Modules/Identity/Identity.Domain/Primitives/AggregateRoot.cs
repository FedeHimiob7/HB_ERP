using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain
{
    public abstract class AggregateRoot<TId>
    {
        private readonly List<DomainEvent> _domainEvents = new();

        public TId Id { get; protected set; } = default!;

        public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;

        protected AggregateRoot() { }
        protected AggregateRoot(TId id)
        {
            Id = id;
        }

        protected void Raise(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
