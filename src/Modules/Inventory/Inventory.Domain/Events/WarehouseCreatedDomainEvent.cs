using HB_ERP.SharedKernel.Domain;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Events
{
    public sealed record WarehouseCreatedDomainEvent(
        WarehouseId WarehouseId,
        ProductServiceLineId ProductServiceLineId,
        string Name) : DomainEvent(WarehouseId.Value);
}
