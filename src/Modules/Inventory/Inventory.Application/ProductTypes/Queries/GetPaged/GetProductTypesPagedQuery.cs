using ErrorOr;
using Inventory.Application.ProductTypes.Models;
using Inventory.Domain.SearchParametersModel;
using MediatR;

namespace Inventory.Application.ProductTypes.Queries.GetPaged
{
    public record GetProductTypesPagedQuery(ProductTypeFilter Filter) : IRequest<ErrorOr<PagedProductTypesResult>>;
}
