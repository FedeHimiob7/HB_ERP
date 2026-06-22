using ErrorOr;
using MediatR;

namespace Inventory.Application.ProductTypes.Commands.DeactivateProductType
{
    public record DeactivateProductTypeCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
