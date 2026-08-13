using Identity.Application.EventLogs.Queries.GetEventLogPaged;
using Identity.Domain.Entities;
using Identity.Domain.Enums;
using Identity.Domain.Repositories;
using NSubstitute;

namespace Application.Tests
{
    // Handler enteramente nuevo de F0 — expone GET /api/EventLogs/security/paged
    // (ver convención de EventLogsController único cross-módulo en FISCAL_ROADMAP.md).
    public sealed class GetEventLogPagedQueryHandlerTests
    {
        private readonly IEventLogRepository _repository = Substitute.For<IEventLogRepository>();

        private GetEventLogPagedQueryHandler CreateHandler() => new(_repository);

        [Fact]
        public async Task Handle_PassesAllFiltersToRepositoryAndMapsResponse()
        {
            var userId = Guid.NewGuid();
            var from = new DateTime(2026, 8, 1);
            var to = new DateTime(2026, 8, 10);
            var eventLog = EventLog.Create(EventLogType.LoginSucceeded, new DateTime(2026, 8, 5), "Login exitoso", userId: userId);

            _repository.GetPagedAsync(1, 10, EventLogType.LoginSucceeded, userId, from, to, Arg.Any<CancellationToken>())
                .Returns((new List<EventLog> { eventLog }, 5));

            var query = new GetEventLogPagedQuery(1, 10, EventLogType.LoginSucceeded, userId, from, to);
            var result = await CreateHandler().Handle(query, CancellationToken.None);

            Assert.False(result.IsError);
            Assert.Single(result.Value.Items);
            Assert.Equal(5, result.Value.TotalCount);
            // TypeName es el string legible del enum, no solo el número — mismo criterio que TaxTypeName.
            Assert.Equal("LoginSucceeded", result.Value.Items[0].TypeName);
        }
    }
}
