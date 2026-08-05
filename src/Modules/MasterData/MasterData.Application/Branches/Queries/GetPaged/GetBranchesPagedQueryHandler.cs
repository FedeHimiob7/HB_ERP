using ErrorOr;
using MasterData.Application.Branches.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.SearchParametersModel;
using MediatR;

namespace MasterData.Application.Branches.Queries.GetPaged
{
    internal sealed class GetBranchesPagedQueryHandler : IRequestHandler<GetBranchesPagedQuery, ErrorOr<PagedBranchesResult>>
    {
        private readonly IBranchRepository _repository;

        public GetBranchesPagedQueryHandler(IBranchRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedBranchesResult>> Handle(GetBranchesPagedQuery request, CancellationToken cancellationToken)
        {
            var filter = new BranchFilter(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm
            );

            var (branches, totalCount) = await _repository.GetPagedAsync(filter, cancellationToken);

            var mappedItems = branches.Select(b => new BranchResponse(
                b.Id.Value,
                b.CompanyId.Value,
                b.Name,
                b.Address,
                b.SequenceNumber
            )).ToList();

            return new PagedBranchesResult(mappedItems, totalCount);
        }
    }
}
