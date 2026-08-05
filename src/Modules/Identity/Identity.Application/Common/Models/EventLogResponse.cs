namespace Identity.Application.Common.Models
{
    public record EventLogResponse(
        Guid Id,
        int Type,
        string TypeName,
        DateTime OccurredAt,
        Guid? UserId,
        string? AttemptedEmail,
        string? EntityType,
        Guid? EntityId,
        string Description);
}
