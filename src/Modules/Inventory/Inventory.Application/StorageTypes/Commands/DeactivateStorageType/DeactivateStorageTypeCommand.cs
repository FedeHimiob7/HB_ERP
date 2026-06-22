using ErrorOr;
using MediatR;

namespace Inventory.Application.StorageTypes.Commands.DeactivateStorageType
{
    public record DeactivateStorageTypeCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
