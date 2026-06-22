using ErrorOr;
using MediatR;

namespace Inventory.Application.ProductTypes.Commands.CreateProductType
{
    public record CreateProductTypeCommand(string Name, string? Description) : IRequest<ErrorOr<Guid>>;
}
