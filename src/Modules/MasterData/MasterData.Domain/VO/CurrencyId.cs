using HB_ERP.SharedKernel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.VO
{
    public readonly record struct CurrencyId(Guid Value)
    {
        public static CurrencyId New()
            => new CurrencyId(Helper.GetNewCombSequentialID());

        public static CurrencyId Create(Guid id)
            => new CurrencyId(id);
    }
}

