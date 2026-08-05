using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.Enums;
using MasterData.Domain.VO;

namespace MasterData.Domain.Events
{
    public sealed record TaxCreatedDomainEvent(
        TaxId TaxId,
        string Name,
        TaxType TaxType) : DomainEvent(TaxId.Value);
}
