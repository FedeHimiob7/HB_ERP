using ErrorOr;
using Inventory.Application.StorageTypes.Models;
using MediatR;

namespace Inventory.Application.StorageTypes.Queries.GetById
{
    public record GetStorageTypeByIdQuery(Guid Id) : IRequest<ErrorOr<StorageTypeResponse>>;
}
