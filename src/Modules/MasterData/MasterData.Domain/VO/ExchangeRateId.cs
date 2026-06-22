using HB_ERP.SharedKernel.Domain.Common;

namespace MasterData.Domain.VO
{
    public readonly record struct ExchangeRateId(Guid Value)
    {
        public static ExchangeRateId New()
            => new ExchangeRateId(Helper.GetNewCombSequentialID());

        public static ExchangeRateId Create(Guid id)
            => new ExchangeRateId(id);
    }
}
