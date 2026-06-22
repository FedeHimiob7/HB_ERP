using HB_ERP.SharedKernel.Domain;
using Inventory.Domain.VO;

namespace Inventory.Domain.Events
{
    public sealed record ProductSubCategoryCreatedDomainEvent(
        ProductSubCategoryId ProductSubCategoryId,
        ProductCategoryId ProductCategoryId,
        string Name) : DomainEvent(ProductSubCategoryId.Value);
}
