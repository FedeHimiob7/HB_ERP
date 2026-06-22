using ErrorOr;
using MediatR;

namespace Inventory.Application.ProductBrands.Commands.DeactivateProductBrand
{
    public record DeactivateProductBrandCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
