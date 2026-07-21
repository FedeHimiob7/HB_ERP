using ErrorOr;
using Inventory.Application.Products.Models;
using MediatR;

namespace Inventory.Application.Products.Queries.GetById
{
    public record GetProductByIdQuery(Guid Id) : IRequest<ErrorOr<ProductResponse>>;
}
