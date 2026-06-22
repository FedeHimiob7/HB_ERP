using ErrorOr;
using Inventory.Application.StorageTypes.Models;
using MediatR;

namespace Inventory.Application.StorageTypes.Queries.GetAll
{
    public record GetAllStorageTypesQuery() : IRequest<ErrorOr<IReadOnlyList<StorageTypeResponse>>>;
}
