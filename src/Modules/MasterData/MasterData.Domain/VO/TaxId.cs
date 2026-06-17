using HB_ERP.SharedKernel.Domain.Common;

namespace MasterData.Domain.VO
{
    public readonly record struct TaxId(Guid Value)
    {
        public static TaxId New() => new TaxId(Helper.GetNewCombSequentialID());
        public static TaxId Create(Guid id) => new TaxId(id);
    }
}
