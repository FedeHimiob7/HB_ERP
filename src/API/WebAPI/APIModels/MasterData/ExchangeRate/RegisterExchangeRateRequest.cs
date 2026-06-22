using MasterData.Domain.Enums;

namespace WebAPI.APIModels.MasterData.ExchangeRate
{
    public record RegisterExchangeRateRequest(decimal Rate, ExchangeRateSource Source);
}
