using ErrorOr;
using Inventory.Application.ProductCategories.Models;
using Inventory.Domain.SearchParametersModel;
using MediatR;

namespace Inventory.Application.ProductCategories.Queries.GetPaged
{
    public record GetProductCategoriesPagedQuery(ProductCategoryFilter Filter) : IRequest<ErrorOr<PagedProductCategoriesResult>>;
}
