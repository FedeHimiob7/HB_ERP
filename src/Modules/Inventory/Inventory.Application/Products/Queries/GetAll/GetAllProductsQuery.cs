using ErrorOr;
using Inventory.Application.Products.Models;
using MediatR;

namespace Inventory.Application.Products.Queries.GetAll
{
    public record GetAllProductsQuery(Guid? ProductServiceLineId = null) : IRequest<ErrorOr<IReadOnlyList<ProductResponse>>>;
}
