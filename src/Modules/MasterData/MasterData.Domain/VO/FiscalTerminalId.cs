using HB_ERP.SharedKernel.Domain.Common;
using System;

namespace MasterData.Domain.VO
{
    public readonly record struct FiscalTerminalId(Guid Value)
    {
        public static FiscalTerminalId New()
            => new FiscalTerminalId(Helper.GetNewCombSequentialID());

        public static FiscalTerminalId Create(Guid id)
            => new FiscalTerminalId(id);
    }
}
