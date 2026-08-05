using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.VO;

namespace MasterData.Domain.Events
{
    public sealed record BranchCreatedDomainEvent(
        BranchId BranchId,
        CompanyId CompanyId,
        string Name,
        int SequenceNumber) : DomainEvent(BranchId.Value);
}
