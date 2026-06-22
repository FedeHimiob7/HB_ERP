using ErrorOr;
using Inventory.Application.StorageTypes.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.StorageTypes.Queries.GetById
{
    internal sealed class GetStorageTypeByIdQueryHandler : IRequestHandler<GetStorageTypeByIdQuery, ErrorOr<StorageTypeResponse>>
    {
        private readonly IStorageTypeRepository _repository;
        public GetStorageTypeByIdQueryHandler(IStorageTypeRepository repository) => _repository = repository;

        public async Task<ErrorOr<StorageTypeResponse>> Handle(GetStorageTypeByIdQuery request, CancellationToken cancellationToken)
        {
            var storageType = await _repository.GetByIdAsync(StorageTypeId.Create(request.Id), cancellationToken);
            if (storageType is null) return StorageTypeErrors.NotFound;

            return new StorageTypeResponse(storageType.Id.Value, storageType.Name, storageType.Description);
        }
    }
}
