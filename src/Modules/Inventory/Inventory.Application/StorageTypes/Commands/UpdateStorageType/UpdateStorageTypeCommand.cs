using ErrorOr;
using Inventory.Application.StorageTypes.Models;
using MediatR;

namespace Inventory.Application.StorageTypes.Commands.UpdateStorageType
{
    public record UpdateStorageTypeCommand(Guid Id, string Name, string? Description) : IRequest<ErrorOr<StorageTypeResponse>>;
}
