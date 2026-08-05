using Identity.Domain.Enums;

namespace WebAPI.APIModels.Identity.EventLog
{
    public record GetEventLogPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        EventLogType? Type = null,
        Guid? UserId = null,
        DateTime? From = null,
        DateTime? To = null
    );
}
