using ErrorOr;
using Inventory.Application.ProductSubCategories.Models;
using MediatR;

namespace Inventory.Application.ProductSubCategories.Queries.GetById
{
    public record GetProductSubCategoryByIdQuery(Guid Id) : IRequest<ErrorOr<ProductSubCategoryResponse>>;
}
