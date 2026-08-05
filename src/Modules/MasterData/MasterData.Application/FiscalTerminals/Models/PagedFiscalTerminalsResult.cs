namespace MasterData.Application.FiscalTerminals.Models
{
    public record PagedFiscalTerminalsResult(IReadOnlyList<FiscalTerminalResponse> Items, int TotalCount);
}
