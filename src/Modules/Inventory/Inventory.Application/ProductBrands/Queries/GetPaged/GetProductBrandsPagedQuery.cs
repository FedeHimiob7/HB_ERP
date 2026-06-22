using ErrorOr;
using Inventory.Application.ProductBrands.Models;
using Inventory.Domain.SearchParametersModel;
using MediatR;

namespace Inventory.Application.ProductBrands.Queries.GetPaged
{
    public record GetProductBrandsPagedQuery(ProductBrandFilter Filter) : IRequest<ErrorOr<PagedProductBrandsResult>>;
}
