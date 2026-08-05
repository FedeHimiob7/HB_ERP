namespace WebAPI.APIModels.MasterData.FiscalTerminal
{
    public record GetFiscalTerminalsPagedRequest(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? BranchId = null,
    string? SearchTerm = null
);
}
