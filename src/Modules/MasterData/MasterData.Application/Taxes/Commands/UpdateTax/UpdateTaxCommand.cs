using ErrorOr;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.Enums;
using MediatR;

namespace MasterData.Application.Taxes.Commands.UpdateTax
{
    public record UpdateTaxCommand(Guid Id, string Name, TaxType TaxType, decimal Rate) : IRequest<ErrorOr<TaxResponse>>;
}
