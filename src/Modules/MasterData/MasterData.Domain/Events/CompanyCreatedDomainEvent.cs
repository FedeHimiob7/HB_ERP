using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.VO;

namespace MasterData.Domain.Events
{
    public sealed record CompanyCreatedDomainEvent(
        CompanyId CompanyId,
        string Rif,
        string LegalName) : DomainEvent(CompanyId.Value);
}
