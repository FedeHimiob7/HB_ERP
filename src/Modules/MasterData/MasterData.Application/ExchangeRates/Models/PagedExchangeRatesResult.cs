namespace MasterData.Application.ExchangeRates.Models
{
    public record PagedExchangeRatesResult(
        IReadOnlyList<ExchangeRateResponse> Items,
        int TotalCount,
        int PageNumber,
        int PageSize);
}
