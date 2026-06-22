using ErrorOr;
using HB_ERP.SharedKernel.Domain;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Interfaces;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Warehouses.Commands.CreateWarehouse
{
    internal sealed class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, ErrorOr<Guid>>
    {
        private readonly IWarehouseRepository _repository;
        private readonly IProductServiceLineRepository _pslRepository;
        private readonly IInventoryUnitOfWork _unitOfWork;
        private readonly ICurrentUserProvider _currentUser;

        public CreateWarehouseCommandHandler(
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

        public async Task<ErrorOr<Guid>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
        {
            if (!_currentUser.PslIds.Contains(request.ProductServiceLineId))
                return CommonErrors.PslAccessDenied;

            var pslId = ProductServiceLineId.Create(request.ProductServiceLineId);

            if (await _pslRepository.GetByIdAsync(pslId, cancellationToken) is null)
                return WarehouseErrors.InvalidProductServiceLine;

            if (await _repository.ExistsByNameInPslAsync(request.Name, pslId, cancellationToken: cancellationToken))
                return WarehouseErrors.DuplicateNameInPsl;

            var result = Warehouse.Create(pslId, request.Name, request.Description, request.Latitude, request.Longitude);
            if (result.IsError) return result.Errors;

            await _repository.AddAsync(result.Value, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return result.Value.Id.Value;
        }
    }
}
