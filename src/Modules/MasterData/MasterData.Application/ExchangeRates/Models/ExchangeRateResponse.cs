using MasterData.Domain.Enums;

namespace MasterData.Application.ExchangeRates.Models
{
    public record ExchangeRateResponse(
        Guid Id,
        DateTime RegisterDate,
        decimal Rate,
        ExchangeRateSource Source,
        string SourceName);
}
