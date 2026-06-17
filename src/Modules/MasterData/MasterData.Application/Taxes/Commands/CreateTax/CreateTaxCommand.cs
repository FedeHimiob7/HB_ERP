using ErrorOr;
using MasterData.Domain.Enums;
using MediatR;

namespace MasterData.Application.Taxes.Commands.CreateTax
{
    public record CreateTaxCommand(string Name, TaxType TaxType, decimal Rate) : IRequest<ErrorOr<Guid>>;
}
