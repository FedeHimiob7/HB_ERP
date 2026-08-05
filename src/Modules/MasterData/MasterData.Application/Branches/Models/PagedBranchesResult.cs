namespace MasterData.Application.Branches.Models
{
    public record PagedBranchesResult(IReadOnlyList<BranchResponse> Items, int TotalCount);
}
