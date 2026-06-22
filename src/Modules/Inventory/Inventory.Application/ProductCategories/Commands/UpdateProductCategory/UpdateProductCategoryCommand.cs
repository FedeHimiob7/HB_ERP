using ErrorOr;
using Inventory.Application.ProductCategories.Models;
using MediatR;

namespace Inventory.Application.ProductCategories.Commands.UpdateProductCategory
{
    public record UpdateProductCategoryCommand(
        Guid Id,
        Guid ProductServiceLineId,
        Guid? ProductTypeId,
        string Name,
        string? Description) : IRequest<ErrorOr<ProductCategoryResponse>>;
}
