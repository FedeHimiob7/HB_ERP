using HB_ERP.SharedKernel.Domain.Common;

namespace MasterData.Domain.VO
{
    public readonly record struct FiscalTaxRateId(Guid Value)
    {
        public static FiscalTaxRateId New() => new FiscalTaxRateId(Helper.GetNewCombSequentialID());
        public static FiscalTaxRateId Create(Guid id) => new FiscalTaxRateId(id);
    }
}
