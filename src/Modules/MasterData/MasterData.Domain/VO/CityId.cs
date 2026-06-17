using HB_ERP.SharedKernel.Domain.Common;

namespace MasterData.Domain.VO
{
    public readonly record struct CityId(Guid Value)
    {
        public static CityId New()
            => new CityId(Helper.GetNewCombSequentialID());

        public static CityId Create(Guid id)
            => new CityId(id);
    }
}
