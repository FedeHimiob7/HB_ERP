using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories
{
    internal sealed class EventLogRepository : IEventLogRepository
    {
        private readonly IdentityDbContext _dbContext;
        public EventLogRepository(IdentityDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(EventLog eventLog, CancellationToken cancellationToken = default)
            => await _dbContext.EventLogs.AddAsync(eventLog, cancellationToken);

        public async Task<(IReadOnlyList<EventLog> Items, int TotalCount)> GetPagedAsync(
            int pageNumber,
            int pageSize,
            EventLogType? type,
            Guid? userId,
            DateTime? from,
            DateTime? to,
            CancellationToken cancellationToken = default)
        {
            var query = _dbContext.EventLogs.AsNoTracking().AsQueryable();

            if (type is not null)
                query = query.Where(e => e.Type == type);

            if (userId is not null)
                query = query.Where(e => e.UserId == userId);

            if (from is not null)
                query = query.Where(e => e.OccurredAt >= from);

            if (to is not null)
                query = query.Where(e => e.OccurredAt <= to);

            query = query.OrderByDescending(e => e.OccurredAt);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
