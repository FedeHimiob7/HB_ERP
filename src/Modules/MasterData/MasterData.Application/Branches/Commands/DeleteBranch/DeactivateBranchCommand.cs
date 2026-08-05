using ErrorOr;
using MediatR;

namespace MasterData.Application.Branches.Commands.DeleteBranch
{
    public record DeactivateBranchCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
