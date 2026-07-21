using ErrorOr;
using Inventory.Application.Products.Models;
using MediatR;

namespace Inventory.Application.Products.Commands.GenerateProductCode
{
    public record GenerateProductCodeCommand(Guid ProductServiceLineId) : IRequest<ErrorOr<GenerateProductCodeResult>>;
}
