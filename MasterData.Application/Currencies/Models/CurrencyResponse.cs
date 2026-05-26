using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Currencies.Models
{
    public record CurrencyResponse(
    Guid Id,
    string Code,
    string Name,
    string Symbol);
}
