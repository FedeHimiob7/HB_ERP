using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.VO;

namespace MasterData.Domain.Events
{
    public sealed record CityCreatedDomainEvent(
        CityId CityId,
        StateId StateId,
        string Name) : DomainEvent(CityId.Value);
}
