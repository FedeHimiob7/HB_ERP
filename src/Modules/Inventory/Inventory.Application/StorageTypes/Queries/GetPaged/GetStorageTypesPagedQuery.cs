using ErrorOr;
using Inventory.Application.StorageTypes.Models;
using Inventory.Domain.SearchParametersModel;
using MediatR;

namespace Inventory.Application.StorageTypes.Queries.GetPaged
{
    public record GetStorageTypesPagedQuery(StorageTypeFilter Filter) : IRequest<ErrorOr<PagedStorageTypesResult>>;
}
