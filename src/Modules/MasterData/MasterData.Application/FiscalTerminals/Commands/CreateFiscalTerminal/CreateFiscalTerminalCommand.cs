using ErrorOr;
using MasterData.Domain.Enums;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Commands.CreateFiscalTerminal
{
    public record CreateFiscalTerminalCommand(
    Guid BranchId,
    string Name,
    EmissionMethod EmissionMethod
) : IRequest<ErrorOr<Guid>>;
}
