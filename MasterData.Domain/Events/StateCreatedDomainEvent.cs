using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Events
{
    public sealed record StateCreatedDomainEvent(
    StateId StateId,
    CountryId CountryId,
    string Code,
    string Name) : DomainEvent(StateId.Value);
}
