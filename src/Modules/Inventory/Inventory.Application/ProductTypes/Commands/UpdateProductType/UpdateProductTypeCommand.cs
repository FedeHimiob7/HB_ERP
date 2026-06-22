using ErrorOr;
using Inventory.Application.ProductTypes.Models;
using MediatR;

namespace Inventory.Application.ProductTypes.Commands.UpdateProductType
{
    public record UpdateProductTypeCommand(Guid Id, string Name, string? Description) : IRequest<ErrorOr<ProductTypeResponse>>;
}
