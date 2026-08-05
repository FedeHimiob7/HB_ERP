using MasterData.Domain.Enums;

namespace WebAPI.APIModels.MasterData.FiscalTerminal
{
    public record UpdateFiscalTerminalRequest(string Name, EmissionMethod EmissionMethod);
}
