using ErrorOr;
using MasterData.Application.FiscalTerminals.Models;
using MasterData.Domain.Enums;
using MediatR;

namespace MasterData.Application.FiscalTerminals.Commands.UpdateFiscalTerminal
{
    public record UpdateFiscalTerminalCommand(
    Guid Id,
    string Name,
    EmissionMethod EmissionMethod
) : IRequest<ErrorOr<FiscalTerminalResponse>>;
}
