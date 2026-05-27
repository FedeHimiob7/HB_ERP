using HB_ERP.SharedKernel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.VO
{
    public readonly record struct UnitId(Guid Value)
    {
        public static UnitId New() => new UnitId(Helper.GetNewCombSequentialID());
        public static UnitId Create(Guid id) => new UnitId(id);
    }
}
