using HB_ERP.SharedKernel.Domain;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Events
{
    public sealed record ProductCreatedDomainEvent(
        ProductId ProductId,
        ProductServiceLineId ProductServiceLineId,
        string Code,
        string Name) : DomainEvent(ProductId.Value);
}
