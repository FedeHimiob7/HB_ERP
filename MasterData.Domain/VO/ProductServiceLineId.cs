using HB_ERP.SharedKernel.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.VO
{
    public readonly record struct ProductServiceLineId(Guid Value)
    {
        public static ProductServiceLineId New()
            => new ProductServiceLineId(Helper.GetNewCombSequentialID());

        public static ProductServiceLineId Create(Guid id)
            => new ProductServiceLineId(id);
    }
}
