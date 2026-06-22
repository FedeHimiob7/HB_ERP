using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.Warehouses.Commands.DeactivateWarehouse
{
    internal sealed class DeactivateWarehouseCommandHandler : IRequestHandler<DeactivateWarehouseCommand, ErrorOr<Success>>
    {
        private readonly IWarehouseRepository _repository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public DeactivateWarehouseCommandHandler(
            IWarehouseRepository repository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<Success>> Handle(DeactivateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await _repository.GetByIdAsync(WarehouseId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (warehouse is null) return WarehouseErrors.NotFound;

            warehouse.Deactivate();

            await _repository.UpdateAsync(warehouse, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
