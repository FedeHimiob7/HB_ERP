using ErrorOr;
using MasterData.Application.Branches.Models;
using MediatR;

namespace MasterData.Application.Branches.Commands.UpdateBranch
{
    public record UpdateBranchCommand(
    Guid Id,
    string Name,
    string Address
) : IRequest<ErrorOr<BranchResponse>>;
}
