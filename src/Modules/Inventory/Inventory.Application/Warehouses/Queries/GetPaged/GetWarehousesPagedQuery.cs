using ErrorOr;
using Inventory.Application.Warehouses.Models;
using Inventory.Domain.SearchParametersModel;
using MediatR;

namespace Inventory.Application.Warehouses.Queries.GetPaged
{
    public record GetWarehousesPagedQuery(WarehouseFilter Filter) : IRequest<ErrorOr<PagedWarehousesResult>>;
}
