using HB_ERP.SharedKernel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.VO
{
    public readonly record struct CountryId(Guid Value)
    {
        public static CountryId New()
            => new CountryId(Helper.GetNewCombSequentialID());

        public static CountryId Create(Guid id)
            => new CountryId(id);
    }
}
