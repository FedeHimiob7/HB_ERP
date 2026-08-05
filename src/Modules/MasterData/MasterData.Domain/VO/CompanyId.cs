using HB_ERP.SharedKernel.Domain.Common;
using System;

namespace MasterData.Domain.VO
{
    public readonly record struct CompanyId(Guid Value)
    {
        public static readonly CompanyId Singleton = new(Guid.Parse("00000000-0000-0000-0000-000000000001"));

        public static CompanyId New()
            => new CompanyId(Helper.GetNewCombSequentialID());

        public static CompanyId Create(Guid id)
            => new CompanyId(id);
    }
}
