using ErrorOr;
using MediatR;

namespace MasterData.Application.Taxes.Commands.DeactivateTax
{
    public record DeactivateTaxCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
