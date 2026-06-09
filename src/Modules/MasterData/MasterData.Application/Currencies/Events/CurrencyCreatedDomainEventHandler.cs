using HB_ERP.SharedKernel.Infrastructure;
using HB_ERP.SharedKernel.IntegrationEvents;
using HB_ERP.SharedKernel.IntegrationEvents.MasterData.Currency;
using MasterData.Application.Currencies.Commands.CreateCurrencie;
using MasterData.Domain.Events;
using MasterData.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Events
{
    internal sealed class CurrencyCreatedDomainEventHandler
    : INotificationHandler<CurrencyCreatedDomainEvent>
    {
        private readonly IOutboxRepository _outboxRepository;
        private readonly ILogger<CurrencyCreatedDomainEventHandler> _logger;

        public CurrencyCreatedDomainEventHandler(
            IOutboxRepository outboxRepository,
            ILogger<CurrencyCreatedDomainEventHandler> logger)
        {
            _outboxRepository = outboxRepository;
            _logger = logger;
        }

        public async Task Handle(CurrencyCreatedDomainEvent notification, CancellationToken cancellationToken)
        {
            // 1. TRADUCCIÓN: Convertimos el Evento de Dominio al Evento de Integración
            var integrationEvent = new CurrencyCreatedIntegrationEvent(
                notification.CurrencyId.Value, // Sacamos el Guid primitivo
                notification.Code,
                notification.Name
            );

            // 2. Serializamos el EVENTO DE INTEGRACIÓN a JSON
            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                // ✅ CAMBIO: Guardamos el AssemblyQualifiedName para que Reflection lo encuentre exacto
                Type = integrationEvent.GetType().AssemblyQualifiedName!,
                Content = JsonSerializer.Serialize(integrationEvent),
                OccurredOnUtc = DateTime.UtcNow
            };

            await _outboxRepository.AddAsync(outboxMessage, cancellationToken);

            _logger.LogInformation("📦 Evento de Integración {EventType} empaquetado en el Outbox exitosamente.", outboxMessage.Type);
        }
    }
}
