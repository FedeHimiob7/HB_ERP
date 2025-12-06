using Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.VO
{
    public readonly record struct UserId(Guid Value)
    {
        public static UserId New()
        => new UserId(Helper.GetNewCombSequentialID());

        public static UserId Create(Guid id)
        => new UserId(id);
    }
}
