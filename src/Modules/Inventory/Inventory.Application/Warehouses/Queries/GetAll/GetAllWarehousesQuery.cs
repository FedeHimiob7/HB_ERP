using ErrorOr;
using Inventory.Application.Warehouses.Models;
using MediatR;

namespace Inventory.Application.Warehouses.Queries.GetAll
{
    public record GetAllWarehousesQuery(Guid? ProductServiceLineId = null) : IRequest<ErrorOr<IReadOnlyList<WarehouseResponse>>>;
}
