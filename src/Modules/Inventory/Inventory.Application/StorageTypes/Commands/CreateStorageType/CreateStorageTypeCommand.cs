using ErrorOr;
using MediatR;

namespace Inventory.Application.StorageTypes.Commands.CreateStorageType
{
    public record CreateStorageTypeCommand(string Name, string? Description) : IRequest<ErrorOr<Guid>>;
}
