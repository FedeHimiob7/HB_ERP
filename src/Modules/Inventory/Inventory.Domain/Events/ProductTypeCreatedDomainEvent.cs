using HB_ERP.SharedKernel.Domain;
using Inventory.Domain.VO;

namespace Inventory.Domain.Events
{
    public sealed record ProductTypeCreatedDomainEvent(
        ProductTypeId ProductTypeId,
        string Name) : DomainEvent(ProductTypeId.Value);
}
