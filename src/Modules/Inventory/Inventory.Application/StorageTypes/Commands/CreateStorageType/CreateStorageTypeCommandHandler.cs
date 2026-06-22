using ErrorOr;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.StorageTypes.Commands.CreateStorageType
{
    internal sealed class CreateStorageTypeCommandHandler : IRequestHandler<CreateStorageTypeCommand, ErrorOr<Guid>>
    {
        private readonly IStorageTypeRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;

        public CreateStorageTypeCommandHandler(IStorageTypeRepository repository, IInventoryUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ErrorOr<Guid>> Handle(CreateStorageTypeCommand request, CancellationToken cancellationToken)
        {
            if (await _repository.ExistsByNameAsync(request.Name, cancellationToken: cancellationToken))
                return StorageTypeErrors.DuplicateName;

            var result = StorageType.Create(request.Name, request.Description);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
