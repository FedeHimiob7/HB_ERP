using ErrorOr;
using MasterData.Domain.Enums;
using MediatR;

namespace MasterData.Application.Companies.Commands.CreateCompany
{
    public record CreateCompanyCommand(
        string Rif,
        string LegalName,
        string RegisteredAddress,
        TaxpayerType TaxpayerType
    ) : IRequest<ErrorOr<Guid>>;
}
