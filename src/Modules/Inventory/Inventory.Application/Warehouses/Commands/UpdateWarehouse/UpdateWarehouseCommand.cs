using ErrorOr;
using Inventory.Application.Warehouses.Models;
using MediatR;

namespace Inventory.Application.Warehouses.Commands.UpdateWarehouse
{
    public record UpdateWarehouseCommand(
        Guid Id,
        Guid ProductServiceLineId,
        string Name,
        string? Description,
        string? Latitude,
        string? Longitude) : IRequest<ErrorOr<WarehouseResponse>>;
}
