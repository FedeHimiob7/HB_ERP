using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.IntegrationEvents.MasterData.ProductServiceLine
{
    public record ProductServiceLineCreatedIntegrationEvent(
    Guid ProductServiceLineId,
    string Description,
    string Name,
    string CreatedBy);
}
