using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Inventory.Infrastructure.Persistence
{
    public sealed class InventoryOutboxPublisher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<InventoryOutboxPublisher> _logger;

        public InventoryOutboxPublisher(IServiceScopeFactory scopeFactory, ILogger<InventoryOutboxPublisher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("InventoryOutboxPublisher iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    var messages = await dbContext.OutboxMessages
                        .Where(m => m.ProcessedAtUtc == null)
                        .OrderBy(m => m.OccurredOnUtc)
                        .Take(20)
                        .ToListAsync(stoppingToken);

                    if (messages.Any())
                    {
                        foreach (var message in messages)
                        {
                            try
                            {
                                Type? eventType = Type.GetType(message.Type);

                                if (eventType != null)
                                {
                                    object? integrationEvent = JsonSerializer.Deserialize(message.Content, eventType);
                                    if (integrationEvent != null)
                                    {
                                        await publishEndpoint.Publish(integrationEvent, stoppingToken);
                                        message.ProcessedAtUtc = DateTime.UtcNow;
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("No se pudo resolver el tipo de evento: {EventType}", message.Type);
                                    message.Error = "Tipo de evento no encontrado";
                                    message.ProcessedAtUtc = DateTime.UtcNow;
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error procesando el mensaje Outbox {MessageId}", message.Id);
                                message.Error = ex.Message;
                            }
                        }

                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error crítico en InventoryOutboxPublisher.");
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
