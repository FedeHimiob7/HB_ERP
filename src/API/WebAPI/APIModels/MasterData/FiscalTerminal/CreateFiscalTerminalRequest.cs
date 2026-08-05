using MasterData.Domain.Enums;

namespace WebAPI.APIModels.MasterData.FiscalTerminal
{
    public record CreateFiscalTerminalRequest(Guid BranchId, string Name, EmissionMethod EmissionMethod);
}
