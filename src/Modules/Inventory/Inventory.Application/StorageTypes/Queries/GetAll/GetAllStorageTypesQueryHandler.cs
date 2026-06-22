using ErrorOr;
using Inventory.Application.StorageTypes.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.StorageTypes.Queries.GetAll
{
    internal sealed class GetAllStorageTypesQueryHandler : IRequestHandler<GetAllStorageTypesQuery, ErrorOr<IReadOnlyList<StorageTypeResponse>>>
    {
        private readonly IStorageTypeRepository _repository;
        public GetAllStorageTypesQueryHandler(IStorageTypeRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<StorageTypeResponse>>> Handle(GetAllStorageTypesQuery request, CancellationToken cancellationToken)
        {
            var items = await _repository.GetAllAsync(cancellationToken);
            return items.Select(x => new StorageTypeResponse(x.Id.Value, x.Name, x.Description)).ToList();
        }
    }
}
