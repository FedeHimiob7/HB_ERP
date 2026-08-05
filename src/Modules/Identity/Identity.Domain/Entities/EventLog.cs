using HB_ERP.SharedKernel.Domain.Primitives;
using Identity.Domain.Enums;
using Identity.Domain.VO;

namespace Identity.Domain.Entities
{
    public sealed class EventLog : AggregateRoot<EventLogId>
    {
        private EventLog() { }

        private EventLog(
            EventLogId id,
            EventLogType type,
            DateTime occurredAt,
            Guid? userId,
            string? attemptedEmail,
            string? entityType,
            Guid? entityId,
            string description)
            : base(id)
        {
            Type = type;
            OccurredAt = occurredAt;
            UserId = userId;
            AttemptedEmail = attemptedEmail;
            EntityType = entityType;
            EntityId = entityId;
            Description = description;
        }

        public EventLogType Type { get; private set; }
        public DateTime OccurredAt { get; private set; }
        public Guid? UserId { get; private set; }
        public string? AttemptedEmail { get; private set; }
        public string? EntityType { get; private set; }
        public Guid? EntityId { get; private set; }
        public string Description { get; private set; } = null!;

        public static EventLog Create(
            EventLogType type,
            DateTime occurredAt,
            string description,
            Guid? userId = null,
            string? attemptedEmail = null,
            string? entityType = null,
            Guid? entityId = null)
            => new(EventLogId.New(), type, occurredAt, userId, attemptedEmail, entityType, entityId, description);
    }
}
