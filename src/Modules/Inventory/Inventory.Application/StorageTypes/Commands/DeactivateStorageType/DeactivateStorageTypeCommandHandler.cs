using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.StorageTypes.Commands.DeactivateStorageType
{
    internal sealed class DeactivateStorageTypeCommandHandler : IRequestHandler<DeactivateStorageTypeCommand, ErrorOr<Success>>
    {
        private readonly IStorageTypeRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public DeactivateStorageTypeCommandHandler(IStorageTypeRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateStorageTypeCommand request, CancellationToken cancellationToken)
        {
            var storageType = await _repository.GetByIdAsync(StorageTypeId.Create(request.Id), cancellationToken);
            if (storageType is null) return StorageTypeErrors.NotFound;

            storageType.Deactivate();

            await _repository.UpdateAsync(storageType, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
