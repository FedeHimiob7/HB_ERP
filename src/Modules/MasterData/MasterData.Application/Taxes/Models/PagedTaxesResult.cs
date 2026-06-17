namespace MasterData.Application.Taxes.Models
{
    public record PagedTaxesResult(IReadOnlyList<TaxResponse> Items, int TotalCount);
}
