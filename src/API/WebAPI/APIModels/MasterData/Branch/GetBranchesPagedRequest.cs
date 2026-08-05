namespace WebAPI.APIModels.MasterData.Branch
{
    public record GetBranchesPagedRequest(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null
);
}
