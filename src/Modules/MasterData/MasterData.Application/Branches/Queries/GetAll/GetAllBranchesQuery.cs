using ErrorOr;
using MasterData.Application.Branches.Models;
using MediatR;

namespace MasterData.Application.Branches.Queries.GetAll
{
    public record GetAllBranchesQuery : IRequest<ErrorOr<IReadOnlyList<BranchResponse>>>;
}
