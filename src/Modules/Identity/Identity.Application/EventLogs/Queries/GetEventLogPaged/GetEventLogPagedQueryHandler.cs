using HB_ERP.SharedKernel.Application.Pagination;
using Identity.Application.Common.Models;
using Identity.Domain.Repositories;

namespace Identity.Application.EventLogs.Queries.GetEventLogPaged
{
    public sealed class GetEventLogPagedQueryHandler(IEventLogRepository eventLogRepository)
        : IRequestHandler<GetEventLogPagedQuery, ErrorOr<PagedList<EventLogResponse>>>
    {
        public async Task<ErrorOr<PagedList<EventLogResponse>>> Handle(
            GetEventLogPagedQuery request,
            CancellationToken cancellationToken)
        {
            var (items, totalCount) = await eventLogRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.Type,
                request.UserId,
                request.From,
                request.To,
                cancellationToken);

            var responses = items.Select(e => new EventLogResponse(
                e.Id.Value,
                (int)e.Type,
                e.Type.ToString(),
                e.OccurredAt,
                e.UserId,
                e.AttemptedEmail,
                e.EntityType,
                e.EntityId,
                e.Description)).ToList();

            return new PagedList<EventLogResponse>(responses, totalCount, request.PageNumber, request.PageSize);
        }
    }
}
