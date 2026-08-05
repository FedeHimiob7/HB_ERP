using MasterData.Domain.Enums;

namespace WebAPI.APIModels.MasterData.Tax
{
    public record UpdateTaxRequest(string Name, TaxType TaxType);
}
