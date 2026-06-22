using ErrorOr;
using Inventory.Application.Warehouses.Models;
using MediatR;

namespace Inventory.Application.Warehouses.Queries.GetById
{
    public record GetWarehouseByIdQuery(Guid Id) : IRequest<ErrorOr<WarehouseResponse>>;
}
