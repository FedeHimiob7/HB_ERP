using HB_ERP.SharedKernel.Domain;
using Inventory.Domain.VO;

namespace Inventory.Domain.Events
{
    public sealed record ProductBrandCreatedDomainEvent(
        ProductBrandId ProductBrandId,
        string Name) : DomainEvent(ProductBrandId.Value);
}
