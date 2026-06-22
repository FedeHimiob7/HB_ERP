using HB_ERP.SharedKernel.Domain;
using Inventory.Domain.VO;
using MasterData.Domain.VO;

namespace Inventory.Domain.Events
{
    public sealed record ProductCategoryCreatedDomainEvent(
        ProductCategoryId ProductCategoryId,
        ProductServiceLineId ProductServiceLineId,
        string Name) : DomainEvent(ProductCategoryId.Value);
}
