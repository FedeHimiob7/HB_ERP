using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Application.ExchangeRates.Commands.SyncFromBCV;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MasterData.Infrastructure.BackgroundServices
{
    internal sealed class BCVRateSyncWorker : BackgroundService
    {
        // 12:00 y 18:00 hora Venezuela (no la hora local del servidor, que puede correr en UTC)
        private static readonly TimeOnly[] ScheduledTimes = [new(12, 0), new(18, 0)];

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IFiscalClock _fiscalClock;
        private readonly ILogger<BCVRateSyncWorker> _logger;

        public BCVRateSyncWorker(IServiceScopeFactory scopeFactory, IFiscalClock fiscalClock, ILogger<BCVRateSyncWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _fiscalClock = fiscalClock;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = CalculateDelayToNext();
                var nextRun = _fiscalClock.VenezuelaNow.Add(delay);

                _logger.LogInformation("BCVRateSyncWorker: próxima sincronización a las {NextRun:HH:mm} ({Delay:hh\\:mm} horas).",
                    nextRun, delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Apagado normal del host — no es un error.
                    break;
                }

                await SyncAsync(stoppingToken);
            }
        }

        private async Task SyncAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("BCVRateSyncWorker: iniciando sincronización programada.");

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<ISender>();

                var result = await sender.Send(new SyncExchangeRateFromBCVCommand(), cancellationToken);

                if (result.IsError)
                    _logger.LogWarning("BCVRateSyncWorker: sincronización fallida — {Error}.", result.FirstError.Description);
                else if (result.Value.WasCreated)
                    _logger.LogInformation("BCVRateSyncWorker: nueva tasa guardada — {Rate} Bs/USD.", result.Value.Rate);
                else
                    _logger.LogInformation("BCVRateSyncWorker: tasa sin cambios ({Rate} Bs/USD), no se insertó nuevo registro.", result.Value.Rate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BCVRateSyncWorker: error inesperado durante la sincronización.");
            }
        }

        private TimeSpan CalculateDelayToNext()
        {
            var now = _fiscalClock.VenezuelaNow;
            var nowTime = TimeOnly.FromDateTime(now);

            // Buscar el próximo slot de hoy que aún no pasó
            var nextToday = ScheduledTimes
                .Where(t => t > nowTime)
                .Order()
                .Select(t => DateOnly.FromDateTime(now).ToDateTime(t))
                .FirstOrDefault();

            if (nextToday != default)
                return nextToday - now;

            // Todos los slots de hoy pasaron → el primero de mañana
            var firstTomorrow = DateOnly.FromDateTime(now).AddDays(1)
                .ToDateTime(ScheduledTimes.Min());

            return firstTomorrow - now;
        }
    }
}
