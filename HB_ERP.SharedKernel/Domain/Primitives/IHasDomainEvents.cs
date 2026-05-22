

namespace HB_ERP.SharedKernel.Domain.Primitives
{
    public interface IHasDomainEvents
    {
        IReadOnlyList<DomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
