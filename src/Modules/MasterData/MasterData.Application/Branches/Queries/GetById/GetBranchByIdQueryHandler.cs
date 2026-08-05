using ErrorOr;
using MasterData.Application.Branches.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Branches.Queries.GetById
{
    internal sealed class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, ErrorOr<BranchResponse>>
    {
        private readonly IBranchRepository _repository;

        public GetBranchByIdQueryHandler(IBranchRepository repository) => _repository = repository;

        public async Task<ErrorOr<BranchResponse>> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
        {
            var branch = await _repository.GetByIdAsync(BranchId.Create(request.Id), cancellationToken);

            if (branch is null) return BranchErrors.NotFound;

            return new BranchResponse(branch.Id.Value, branch.CompanyId.Value, branch.Name, branch.Address, branch.SequenceNumber);
        }
    }
}
