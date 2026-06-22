using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Application.Warehouses.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Warehouses.Commands.UpdateWarehouse
{
    internal sealed class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, ErrorOr<WarehouseResponse>>
    {
        private readonly IWarehouseRepository _repository;
        private readonly IProductServiceLineRepository _pslRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public UpdateWarehouseCommandHandler(
            IWarehouseRepository repository,
            IProductServiceLineRepository pslRepository,
            IInventoryUnitOfWork unitOfWork,
            ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _pslRepository = pslRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<WarehouseResponse>> Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var id = WarehouseId.Create(request.Id);
            var warehouse = await _repository.GetByIdAsync(id, _currentUser.PslIds, cancellationToken);
            if (warehouse is null) return WarehouseErrors.NotFound;

            var pslId = ProductServiceLineId.Create(request.ProductServiceLineId);
            if (await _pslRepository.GetByIdAsync(pslId, cancellationToken) is null)
                return WarehouseErrors.InvalidProductServiceLine;

            if (await _repository.ExistsByNameInPslAsync(request.Name, pslId, excludeId: id, cancellationToken: cancellationToken))
                return WarehouseErrors.DuplicateNameInPsl;

            var updateResult = warehouse.UpdateDetails(pslId, request.Name, request.Description, request.Latitude, request.Longitude);
            if (updateResult.IsError) return updateResult.Errors;

            await _repository.UpdateAsync(warehouse, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new WarehouseResponse(warehouse.Id.Value, warehouse.ProductServiceLineId.Value, warehouse.Name, warehouse.Description, warehouse.Latitude, warehouse.Longitude);
        }
    }
}
