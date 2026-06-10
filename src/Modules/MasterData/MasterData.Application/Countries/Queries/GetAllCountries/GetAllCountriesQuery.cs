using ErrorOr;
using MasterData.Application.Countries.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Queries.GetAllCountries
{
    public record GetAllCountriesQuery() : IRequest<ErrorOr<List<CountryResponse>>>;
}
