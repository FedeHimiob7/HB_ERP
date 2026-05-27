using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Models
{
    public record CountryResponse(
    Guid Id,
    string Name);
}
