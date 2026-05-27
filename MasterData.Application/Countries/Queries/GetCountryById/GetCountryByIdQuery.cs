using ErrorOr;
using MasterData.Application.Countries.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Queries.GetCountryById
{
    public record GetCountryByIdQuery(
    Guid Id
) : IRequest<ErrorOr<CountryResponse>>;
}
