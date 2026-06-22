using HB_ERP.SharedKernel.Domain;
using Inventory.Domain.VO;

namespace Inventory.Domain.Events
{
    public sealed record StorageTypeCreatedDomainEvent(
        StorageTypeId StorageTypeId,
        string Name) : DomainEvent(StorageTypeId.Value);
}
