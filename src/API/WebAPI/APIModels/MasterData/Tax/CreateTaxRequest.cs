using MasterData.Domain.Enums;

namespace WebAPI.APIModels.MasterData.Tax
{
    public record CreateTaxRequest(string Name, TaxType TaxType, decimal Rate);
}
