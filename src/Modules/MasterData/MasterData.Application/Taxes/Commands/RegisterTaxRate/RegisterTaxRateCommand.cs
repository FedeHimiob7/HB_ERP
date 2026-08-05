using ErrorOr;
using MediatR;

namespace MasterData.Application.Taxes.Commands.RegisterTaxRate
{
    public record RegisterTaxRateCommand(Guid TaxId, decimal Rate) : IRequest<ErrorOr<Guid>>;
}
