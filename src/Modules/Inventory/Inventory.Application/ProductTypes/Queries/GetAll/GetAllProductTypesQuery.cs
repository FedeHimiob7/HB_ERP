using ErrorOr;
using Inventory.Application.ProductTypes.Models;
using MediatR;

namespace Inventory.Application.ProductTypes.Queries.GetAll
{
    public record GetAllProductTypesQuery() : IRequest<ErrorOr<IReadOnlyList<ProductTypeResponse>>>;
}
