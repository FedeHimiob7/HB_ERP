using ErrorOr;
using MasterData.Application.Taxes.Models;
using MasterData.Domain.Enums;
using MediatR;

namespace MasterData.Application.Taxes.Commands.UpdateTaxDetails
{
    public record UpdateTaxDetailsCommand(Guid Id, string Name, TaxType TaxType) : IRequest<ErrorOr<TaxResponse>>;
}
