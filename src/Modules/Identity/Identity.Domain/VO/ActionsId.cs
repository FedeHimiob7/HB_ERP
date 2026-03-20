using Identity.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.VO
{
    public readonly record struct ActionsId(Guid Value)
    {
        public static ActionsId New()
        => new ActionsId(Helper.GetNewCombSequentialID());

        public static ActionsId Create(Guid id)
        => new ActionsId(id);
    }
}
