using ErrorOr;
using MediatR;

namespace Inventory.Application.Warehouses.Commands.DeactivateWarehouse
{
    public record DeactivateWarehouseCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
