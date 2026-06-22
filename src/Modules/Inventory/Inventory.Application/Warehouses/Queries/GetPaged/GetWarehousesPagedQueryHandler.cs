using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using Inventory.Application.Warehouses.Models;
using Inventory.Domain.Repositories;
using MediatR;

namespace Inventory.Application.Warehouses.Queries.GetPaged
{
    internal sealed class GetWarehousesPagedQueryHandler : IRequestHandler<GetWarehousesPagedQuery, ErrorOr<PagedWarehousesResult>>
    {
        private readonly IWarehouseRepository _repository;
        private readonly ICurrentUserProvider _currentUser;

        public GetWarehousesPagedQueryHandler(IWarehouseRepository repository, ICurrentUserProvider currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<ErrorOr<PagedWarehousesResult>> Handle(GetWarehousesPagedQuery request, CancellationToken cancellationToken)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(request.Filter, _currentUser.PslIds, cancellationToken);
            var mapped = items.Select(x => new WarehouseResponse(x.Id.Value, x.ProductServiceLineId.Value, x.Name, x.Description, x.Latitude, x.Longitude)).ToList();
            return new PagedWarehousesResult(mapped, totalCount);
        }
    }
}
