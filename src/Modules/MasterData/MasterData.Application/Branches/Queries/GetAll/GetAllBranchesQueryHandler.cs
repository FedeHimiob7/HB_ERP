using ErrorOr;
using MasterData.Application.Branches.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Branches.Queries.GetAll
{
    internal sealed class GetAllBranchesQueryHandler
        : IRequestHandler<GetAllBranchesQuery, ErrorOr<IReadOnlyList<BranchResponse>>>
    {
        private readonly IBranchRepository _repository;

        public GetAllBranchesQueryHandler(IBranchRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<BranchResponse>>> Handle(
            GetAllBranchesQuery request,
            CancellationToken cancellationToken)
        {
            var branches = await _repository.GetAllAsync(cancellationToken);

            var response = branches
                .Select(b => new BranchResponse(b.Id.Value, b.CompanyId.Value, b.Name, b.Address, b.SequenceNumber))
                .ToList();

            return response;
        }
    }
}
