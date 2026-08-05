using ErrorOr;
using MasterData.Application.Branches.Models;
using MediatR;

namespace MasterData.Application.Branches.Queries.GetById
{
    public record GetBranchByIdQuery(Guid Id) : IRequest<ErrorOr<BranchResponse>>;
}
