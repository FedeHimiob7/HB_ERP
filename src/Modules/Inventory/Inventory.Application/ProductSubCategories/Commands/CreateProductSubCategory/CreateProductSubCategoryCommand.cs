using ErrorOr;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Commands.CreateProductSubCategory
{
    public record CreateProductSubCategoryCommand(
        Guid ProductCategoryId,
        string Name,
        string? Description) : IRequest<ErrorOr<Guid>>;
}
