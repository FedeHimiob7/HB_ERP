using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.VO;

namespace MasterData.Domain.Events
{
    public sealed record CurrencyCreatedDomainEvent(
    CurrencyId CurrencyId,
    string Code,
    string Name) : DomainEvent(CurrencyId.Value);
}
