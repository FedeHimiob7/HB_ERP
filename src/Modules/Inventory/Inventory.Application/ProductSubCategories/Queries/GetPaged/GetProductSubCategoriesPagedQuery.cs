using ErrorOr;
using Inventory.Application.ProductSubCategories.Models;
using Inventory.Domain.SearchParametersModel;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Queries.GetPaged
{
    public record GetProductSubCategoriesPagedQuery(ProductSubCategoryFilter Filter) : IRequest<ErrorOr<PagedProductSubCategoriesResult>>;
}
