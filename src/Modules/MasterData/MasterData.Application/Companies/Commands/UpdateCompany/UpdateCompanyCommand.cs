using ErrorOr;
using MasterData.Application.Companies.Models;
using MasterData.Domain.Enums;
using MediatR;

namespace MasterData.Application.Companies.Commands.UpdateCompany
{
    public record UpdateCompanyCommand(
        string Rif,
        string LegalName,
        string RegisteredAddress,
        TaxpayerType TaxpayerType
    ) : IRequest<ErrorOr<CompanyResponse>>;
}
