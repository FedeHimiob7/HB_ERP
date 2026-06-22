using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Warehouses.Models;
using Inventory.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace Inventory.Application.Warehouses.Queries.GetAll
{
    internal sealed class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, ErrorOr<IReadOnlyList<WarehouseResponse>>>
    {
        private readonly IWarehouseRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetAllWarehousesQueryHandler(IWarehouseRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<IReadOnlyList<WarehouseResponse>>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
        {
            var pslId = request.ProductServiceLineId.HasValue
                ? ProductServiceLineId.Create(request.ProductServiceLineId.Value)
                : (ProductServiceLineId?)null;

            var items = await _repository.GetAllAsync(_currentUser.PslIds, pslId, cancellationToken);

            return items.Select(x => new WarehouseResponse(x.Id.Value, x.ProductServiceLineId.Value, x.Name, x.Description, x.Latitude, x.Longitude)).ToList();
        }
    }
}
