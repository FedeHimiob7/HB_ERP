using ErrorOr;
using MediatR;

namespace MasterData.Application.ExchangeRates.Commands.SyncFromBCV
{
    public record SyncExchangeRateFromBCVCommand : IRequest<ErrorOr<ExchangeRateSyncResult>>;

    public record ExchangeRateSyncResult(Guid Id, decimal Rate, bool WasCreated);
}
