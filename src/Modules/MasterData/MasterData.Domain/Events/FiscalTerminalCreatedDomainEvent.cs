using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.Enums;
using MasterData.Domain.VO;

namespace MasterData.Domain.Events
{
    public sealed record FiscalTerminalCreatedDomainEvent(
        FiscalTerminalId FiscalTerminalId,
        BranchId BranchId,
        string Name,
        EmissionMethod EmissionMethod) : DomainEvent(FiscalTerminalId.Value);
}
