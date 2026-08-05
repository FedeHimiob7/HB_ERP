using MasterData.Domain.Enums;

namespace WebAPI.APIModels.MasterData.Company
{
    public record UpdateCompanyRequest(string Rif, string LegalName, string RegisteredAddress, TaxpayerType TaxpayerType);
}
