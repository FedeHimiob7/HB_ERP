using ErrorOr;
using MediatR;

namespace Inventory.Application.Products.Commands.DeactivateProduct
{
    public record DeactivateProductCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
