using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Application.StorageTypes.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.StorageTypes.Commands.UpdateStorageType
{
    internal sealed class UpdateStorageTypeCommandHandler : IRequestHandler<UpdateStorageTypeCommand, ErrorOr<StorageTypeResponse>>
    {
        private readonly IStorageTypeRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public UpdateStorageTypeCommandHandler(IStorageTypeRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<StorageTypeResponse>> Handle(UpdateStorageTypeCommand request, CancellationToken cancellationToken)
        {
            var id = StorageTypeId.Create(request.Id);
            var storageType = await _repository.GetByIdAsync(id, cancellationToken);
            if (storageType is null) return StorageTypeErrors.NotFound;

            if (await _repository.ExistsByNameAsync(request.Name, excludeId: id, cancellationToken: cancellationToken))
                return StorageTypeErrors.DuplicateName;

            var updateResult = storageType.UpdateDetails(request.Name, request.Description);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(storageType, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StorageTypeResponse(storageType.Id.Value, storageType.Name, storageType.Description);
        }
    }
}
