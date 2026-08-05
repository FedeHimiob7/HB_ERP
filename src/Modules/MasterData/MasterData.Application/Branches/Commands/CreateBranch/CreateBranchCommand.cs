using ErrorOr;
using MediatR;

namespace MasterData.Application.Branches.Commands.CreateBranch
{
    public record CreateBranchCommand(
    string Name,
    string Address
) : IRequest<ErrorOr<Guid>>;
}
