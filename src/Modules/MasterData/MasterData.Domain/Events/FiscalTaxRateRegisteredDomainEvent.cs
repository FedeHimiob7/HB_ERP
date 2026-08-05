using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.VO;

namespace MasterData.Domain.Events
{
    public sealed record FiscalTaxRateRegisteredDomainEvent(
        FiscalTaxRateId FiscalTaxRateId,
        TaxId TaxId,
        decimal Rate,
        DateTime EffectiveFrom) : DomainEvent(FiscalTaxRateId.Value);
}
