using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Warehouses.Models;
using Inventory.Domain.DomainErrors;
using Inventory.Domain.Repositories;
using Inventory.Domain.VO;
using MediatR;

namespace Inventory.Application.Warehouses.Queries.GetById
{
    internal sealed class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, ErrorOr<WarehouseResponse>>
    {
        private readonly IWarehouseRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetWarehouseByIdQueryHandler(IWarehouseRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<WarehouseResponse>> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
        {
            var warehouse = await _repository.GetByIdAsync(WarehouseId.Create(request.Id), _currentUser.PslIds, cancellationToken);
            if (warehouse is null) return WarehouseErrors.NotFound;

            return new WarehouseResponse(warehouse.Id.Value, warehouse.ProductServiceLineId.Value, warehouse.Name, warehouse.Description, warehouse.Latitude, warehouse.Longitude);
        }
    }
}
