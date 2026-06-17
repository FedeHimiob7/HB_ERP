using MasterData.Domain.Enums;

namespace WebAPI.APIModels.MasterData.Tax
{
    public record GetTaxesPagedRequest(
        int PageNumber = 1,
        int PageSize = 10,
        string? SearchTerm = null,
        TaxType? TaxType = null);
}
