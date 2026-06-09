using HB_ERP.SharedKernel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.VO
{
    public readonly record struct StateId(Guid Value)
    {
        public static StateId New()
            => new StateId(Helper.GetNewCombSequentialID());

        public static StateId Create(Guid id)
            => new StateId(id);
    }
}
