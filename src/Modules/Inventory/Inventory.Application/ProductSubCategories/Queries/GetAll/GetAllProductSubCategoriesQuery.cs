using ErrorOr;
using Inventory.Application.ProductSubCategories.Models;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Queries.GetAll
{
    public record GetAllProductSubCategoriesQuery(Guid? ProductCategoryId = null) : IRequest<ErrorOr<IReadOnlyList<ProductSubCategoryResponse>>>;
}
