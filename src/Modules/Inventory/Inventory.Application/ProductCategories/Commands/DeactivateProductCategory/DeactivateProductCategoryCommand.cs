using ErrorOr;
using MediatR;

namespace Inventory.Application.ProductCategories.Commands.DeactivateProductCategory
{
    public record DeactivateProductCategoryCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
