using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.Enums;
using MasterData.Domain.VO;

namespace MasterData.Domain.Events
{
    public sealed record ExchangeRateRegisteredDomainEvent(
        ExchangeRateId ExchangeRateId,
        DateTime RegisterDate,
        decimal Rate,
        ExchangeRateSource Source) : DomainEvent(ExchangeRateId.Value);
}
