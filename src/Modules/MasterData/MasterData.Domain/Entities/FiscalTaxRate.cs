using ErrorOr;
using HB_ERP.SharedKernel.Domain.Primitives;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Events;
using MasterData.Domain.VO;

namespace MasterData.Domain.Entities
{
    public sealed class FiscalTaxRate : AggregateRoot<FiscalTaxRateId>, IEffectiveDated
    {
        private FiscalTaxRate() { }

        private FiscalTaxRate(FiscalTaxRateId id, TaxId taxId, decimal rate, DateTime effectiveFrom)
            : base(id)
        {
            TaxId = taxId;
            Rate = rate;
            EffectiveFrom = effectiveFrom;
        }

        public TaxId TaxId { get; private set; }
        public decimal Rate { get; private set; }
        public DateTime EffectiveFrom { get; private set; }

        public static ErrorOr<FiscalTaxRate> Create(TaxId taxId, decimal rate, DateTime effectiveFrom)
        {
            if (rate < 0)
                return FiscalTaxRateErrors.RateMustBeNonNegative;

            var fiscalTaxRate = new FiscalTaxRate(FiscalTaxRateId.New(), taxId, rate, effectiveFrom);
            fiscalTaxRate.Raise(new FiscalTaxRateRegisteredDomainEvent(fiscalTaxRate.Id, taxId, rate, effectiveFrom));

            return fiscalTaxRate;
        }
    }
}
