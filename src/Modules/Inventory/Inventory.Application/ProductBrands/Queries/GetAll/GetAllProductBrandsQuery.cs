using ErrorOr;
using Inventory.Application.ProductBrands.Models;
using MediatR;

namespace Inventory.Application.ProductBrands.Queries.GetAll
{
    public record GetAllProductBrandsQuery() : IRequest<ErrorOr<IReadOnlyList<ProductBrandResponse>>>;
}
