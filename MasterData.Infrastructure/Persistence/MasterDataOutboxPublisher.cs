using HB_ERP.SharedKernel.IntegrationEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MasterData.Infrastructure.Persistence
{
    public sealed class MasterDataOutboxPublisher : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MasterDataOutboxPublisher> _logger;

        public MasterDataOutboxPublisher(
            IServiceScopeFactory scopeFactory,
            ILogger<MasterDataOutboxPublisher> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 MasterDataOutboxPublisher iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<MasterDataDbContext>();
                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    // 1. Obtenemos mensajes pendientes
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

                                        // Marcamos como procesado solo si la publicación tuvo éxito
                                        message.ProcessedAtUtc = DateTime.UtcNow;
                                    }
                                }
                                else
                                {
                                    _logger.LogWarning("⚠️ No se pudo resolver el tipo de evento: {EventType}", message.Type);
                                    message.Error = "Tipo de evento no encontrado";
                                    message.ProcessedAtUtc = DateTime.UtcNow; // Marcamos para que no vuelva a intentar si no existe el tipo
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "❌ Error procesando el mensaje Outbox {MessageId}", message.Id);
                                message.Error = ex.Message;
                            }
                        }

                        // 2. Guardamos los cambios de estado (ProcessedAtUtc o errores) en un solo batch
                        await dbContext.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error crítico en el Background Job de Outbox de MasterData.");
                }

                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }

}
