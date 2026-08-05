using Identity.Domain.Entities;
using Identity.Domain.Enums;

namespace Identity.Domain.Repositories
{
    public interface IEventLogRepository
    {
        Task AddAsync(EventLog eventLog, CancellationToken cancellationToken = default);

        Task<(IReadOnlyList<EventLog> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            EventLogType? type,
            Guid? userId,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken = default);
    }
}
