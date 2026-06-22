using ErrorOr;
using Inventory.Application.ProductCategories.Models;
using MediatR;

namespace Inventory.Application.ProductCategories.Queries.GetById
{
    public record GetProductCategoryByIdQuery(Guid Id) : IRequest<ErrorOr<ProductCategoryResponse>>;
}
