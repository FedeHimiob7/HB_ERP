using ErrorOr;
using Inventory.Application.Products.Models;
using Inventory.Domain.SearchParametersModel;
using MediatR;

namespace Inventory.Application.Products.Queries.GetPaged
{
    public record GetProductsPagedQuery(ProductFilter Filter) : IRequest<ErrorOr<PagedProductsResult>>;
}
