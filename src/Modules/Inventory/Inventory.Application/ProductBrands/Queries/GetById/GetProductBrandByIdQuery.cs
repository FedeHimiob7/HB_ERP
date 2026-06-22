using ErrorOr;
using Inventory.Application.ProductBrands.Models;
using MediatR;

namespace Inventory.Application.ProductBrands.Queries.GetById
{
    public record GetProductBrandByIdQuery(Guid Id) : IRequest<ErrorOr<ProductBrandResponse>>;
}
