using ErrorOr;
using Inventory.Application.ProductBrands.Models;
using MediatR;

namespace Inventory.Application.ProductBrands.Commands.UpdateProductBrand
{
    public record UpdateProductBrandCommand(Guid Id, string Name, string? Description) : IRequest<ErrorOr<ProductBrandResponse>>;
}
