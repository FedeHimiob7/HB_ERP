using ErrorOr;
using MediatR;

namespace Inventory.Application.ProductCategories.Commands.CreateProductCategory
{
    public record CreateProductCategoryCommand(
        Guid ProductServiceLineId,
        Guid? ProductTypeId,
        string Name,
        string? Description) : IRequest<ErrorOr<Guid>>;
}
