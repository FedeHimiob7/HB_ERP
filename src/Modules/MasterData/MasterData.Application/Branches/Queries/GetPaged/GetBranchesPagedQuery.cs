using ErrorOr;
using MasterData.Application.Branches.Models;
using MediatR;

namespace MasterData.Application.Branches.Queries.GetPaged
{
    public record GetBranchesPagedQuery(
    int PageNumber,
    int PageSize,
    string? SearchTerm = null
) : IRequest<ErrorOr<PagedBranchesResult>>;
}
