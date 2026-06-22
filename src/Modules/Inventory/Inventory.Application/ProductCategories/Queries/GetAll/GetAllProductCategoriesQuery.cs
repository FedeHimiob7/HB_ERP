using ErrorOr;
using Inventory.Application.ProductCategories.Models;
using MediatR;

namespace Inventory.Application.ProductCategories.Queries.GetAll
{
    public record GetAllProductCategoriesQuery(Guid? ProductServiceLineId = null) : IRequest<ErrorOr<IReadOnlyList<ProductCategoryResponse>>>;
}
