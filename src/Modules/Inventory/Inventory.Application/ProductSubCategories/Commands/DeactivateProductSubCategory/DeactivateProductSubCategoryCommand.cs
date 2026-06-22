using ErrorOr;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Commands.DeactivateProductSubCategory
{
    public record DeactivateProductSubCategoryCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
