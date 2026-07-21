using ErrorOr;
using Inventory.Application.Products.Models;
using MediatR;

namespace Inventory.Application.Products.Queries.GetLastPrice
{
    public record GetProductLastPriceQuery(Guid ProductId) : IRequest<ErrorOr<ProductLastPriceResult?>>;
}
