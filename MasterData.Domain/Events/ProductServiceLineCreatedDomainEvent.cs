using HB_ERP.SharedKernel.Domain;
using MasterData.Domain.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Domain.Events
{
    public sealed record ProductServiceLineCreatedDomainEvent(
    ProductServiceLineId ProductServiceLineId,
    string Description,
    string Name) : DomainEvent(ProductServiceLineId.Value);
}
