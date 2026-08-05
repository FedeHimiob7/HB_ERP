namespace MasterData.Application.Branches.Models
{
    public record BranchResponse(
        Guid Id,
        Guid CompanyId,
        string Name,
        string Address,
        int SequenceNumber);
}
