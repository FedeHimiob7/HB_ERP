using HB_ERP.SharedKernel.Infrastructure;
using MasterData.Application.Currencies.Events;
using MasterData.Domain.Events;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Xunit;

namespace Application.Tests
{
    public sealed class CurrencyCreatedDomainEventHandlerTests
    {
        // Único domain event handler "real" del módulo (traduce CurrencyCreatedDomainEvent a un
        // OutboxMessage) — a diferencia de Identity, donde el patrón equivalente se descartó
        // (ver CLAUDE.md "Domain events — limitación importante en Identity").
        private readonly IOutboxRepository _outboxRepository = Substitute.For<IOutboxRepository>();
        // NullLogger en vez de substitute: Castle no puede proxear ILogger<T> con T `internal`.
        private readonly NullLogger<CurrencyCreatedDomainEventHandler> _logger = NullLogger<CurrencyCreatedDomainEventHandler>.Instance;

        private CurrencyCreatedDomainEventHandler CreateHandler() => new(_outboxRepository, _logger);

        [Fact]
        public async Task Handle_WritesIntegrationEventToOutbox()
        {
            var notification = new CurrencyCreatedDomainEvent(CurrencyId.New(), "USD", "Dólar");

            await CreateHandler().Handle(notification, CancellationToken.None);

            // No nos importa el contenido serializado exacto, sino que efectivamente se haya
            // escrito UN mensaje en el Outbox — el mecanismo de publicación confiable de eventos.
            await _outboxRepository.Received(1).AddAsync(
                Arg.Is<OutboxMessage>(m => m.Content.Contains("USD") && m.Type.Contains("CurrencyCreatedIntegrationEvent")),
                Arg.Any<CancellationToken>());
        }
    }
}
