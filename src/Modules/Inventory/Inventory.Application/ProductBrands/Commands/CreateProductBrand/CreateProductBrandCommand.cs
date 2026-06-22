using ErrorOr;
using MediatR;

namespace Inventory.Application.ProductBrands.Commands.CreateProductBrand
{
    public record CreateProductBrandCommand(string Name, string? Description) : IRequest<ErrorOr<Guid>>;
}
