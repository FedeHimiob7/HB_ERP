using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HB_ERP.SharedKernel.IntegrationEvents.MasterData.Currency
{
    public record CurrencyCreatedIntegrationEvent(Guid CurrencyId, string Code, string CreatedBy);
}
