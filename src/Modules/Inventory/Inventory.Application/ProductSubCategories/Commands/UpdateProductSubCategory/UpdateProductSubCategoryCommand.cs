using ErrorOr;
using Inventory.Application.ProductSubCategories.Models;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Commands.UpdateProductSubCategory
{
    public record UpdateProductSubCategoryCommand(
        Guid Id,
        Guid ProductCategoryId,
        string Name,
        string? Description) : IRequest<ErrorOr<ProductSubCategoryResponse>>;
}
