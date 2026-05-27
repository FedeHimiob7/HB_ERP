using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Models
{
    public record PagedCountriesResult(
    IReadOnlyList<CountryResponse> Items,
    int TotalCount);
}
