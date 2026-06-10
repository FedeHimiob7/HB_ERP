namespace MasterData.Application.Currencies.Models
{
    public record PagedCurrenciesResult(IReadOnlyList<CurrencyResponse> Items, int TotalCount);
}
