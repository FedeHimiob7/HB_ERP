using ErrorOr;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Commands.DeleteFiscalTerminal
{
    public record DeactivateFiscalTerminalCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
