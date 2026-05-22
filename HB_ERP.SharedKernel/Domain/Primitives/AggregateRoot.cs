

namespace HB_ERP.SharedKernel.Domain.Primitives
{
    public abstract class AggregateRoot<TId> : IHasDomainEvents
    {
        private readonly List<DomainEvent> _domainEvents = new();

        public TId Id { get; protected set; } = default!;

        public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

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
