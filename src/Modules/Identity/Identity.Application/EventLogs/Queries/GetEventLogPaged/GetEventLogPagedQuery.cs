using HB_ERP.SharedKernel.Application.Pagination;
using Identity.Application.Common.Models;
using Identity.Domain.Enums;

namespace Identity.Application.EventLogs.Queries.GetEventLogPaged
{
    public record GetEventLogPagedQuery(
        int PageNumber = 1,
        int PageSize = 10,
        EventLogType? Type = null,
        Guid? UserId = null,
        DateTime? From = null,
        DateTime? To = null) : IRequest<ErrorOr<PagedList<EventLogResponse>>>;
}
