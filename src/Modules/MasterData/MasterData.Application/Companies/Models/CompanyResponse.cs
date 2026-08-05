using MasterData.Domain.Enums;

namespace MasterData.Application.Companies.Models
{
    public record CompanyResponse(
        Guid Id,
        string Rif,
        string LegalName,
        string RegisteredAddress,
        TaxpayerType TaxpayerType,
        string TaxpayerTypeName);
}
