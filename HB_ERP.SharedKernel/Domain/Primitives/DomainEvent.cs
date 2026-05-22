using MediatR;

namespace HB_ERP.SharedKernel.Domain
{
    public abstract record DomainEvent(Guid Id) : INotification
    {       
        public DateTime OccurredOnUtc { get; protected set; } = DateTime.UtcNow;
    }
}
