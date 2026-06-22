using ErrorOr;
using MediatR;

namespace Inventory.Application.Warehouses.Commands.CreateWarehouse
{
    public record CreateWarehouseCommand(
        Guid ProductServiceLineId,
        string Name,
        string? Description,
        string? Latitude,
        string? Longitude) : IRequest<ErrorOr<Guid>>;
}
