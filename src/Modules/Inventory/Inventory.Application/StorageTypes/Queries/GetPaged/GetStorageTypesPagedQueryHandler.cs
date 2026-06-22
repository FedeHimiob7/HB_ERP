using ErrorOr;
using Inventory.Application.StorageTypes.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.StorageTypes.Queries.GetPaged
{
    internal sealed class GetStorageTypesPagedQueryHandler : IRequestHandler<GetStorageTypesPagedQuery, ErrorOr<PagedStorageTypesResult>>
    {
        private readonly IStorageTypeRepository _repository;
        public GetStorageTypesPagedQueryHandler(IStorageTypeRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedStorageTypesResult>> Handle(GetStorageTypesPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(request.Filter, cancellationToken);
            var mapped = items.Select(x => new StorageTypeResponse(x.Id.Value, x.Name, x.Description)).ToList();
            return new PagedStorageTypesResult(mapped, totalCount);
        }
    }
}
