using MasterData.Domain.Enums;

namespace MasterData.Application.FiscalTerminals.Models
{
    public record FiscalTerminalResponse(
        Guid Id,
        Guid BranchId,
        string Name,
        EmissionMethod EmissionMethod,
        string EmissionMethodName);
}
