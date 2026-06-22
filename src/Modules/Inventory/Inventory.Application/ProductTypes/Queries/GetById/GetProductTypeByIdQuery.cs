using ErrorOr;
using Inventory.Application.ProductTypes.Models;
using MediatR;

namespace Inventory.Application.ProductTypes.Queries.GetById
{
    public record GetProductTypeByIdQuery(Guid Id) : IRequest<ErrorOr<ProductTypeResponse>>;
}
