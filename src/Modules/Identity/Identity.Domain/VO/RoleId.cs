using Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.VO
{
    public readonly record struct RoleId(Guid Value)
    {
        public static RoleId New()
        => new RoleId(Helper.GetNewCombSequentialID());

        public static RoleId Create(Guid id)
        => new RoleId(id);
    }
}
