using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Persistence
{
    public sealed class IdentityOutboxPublisher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<IdentityOutboxPublisher> _logger;

        public IdentityOutboxPublisher(
            IServiceScopeFactory scopeFactory,
            ILogger<IdentityOutboxPublisher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    // 1. Extraemos los mensajes pendientes del esquema de Identity
                    var messages = await dbContext.OutboxMessages
                        .Where(m => m.ProcessedAtUtc == null)
                        .OrderBy(m => m.OccurredOnUtc)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    foreach (var message in messages)
                    {
                        try
                        {
                            // 2. Mapeo dinámico o por tipo de evento de Identity
                            // (Cuando agregues eventos como UserCreatedIntegrationEvent, los deserializas aquí)

                            // Ejemplo genérico o por string si aún no tienes eventos salientes en Identity:
                            // var integrationEvent = JsonSerializer.Deserialize(message.Content, Type.GetType(message.Type));
                            // await publishEndpoint.Publish(integrationEvent, stoppingToken);

                            message.ProcessedAtUtc = DateTime.UtcNow;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "❌ Error procesando el mensaje Outbox de Identity {MessageId}", message.Id);
                            message.Error = ex.Message;
                        }
                    }

                    if (messages.Any())
                    {
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error crítico en el Background Job de Outbox de Identity.");
                }

                // Escaneo optimizado cada 1 segundo
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
