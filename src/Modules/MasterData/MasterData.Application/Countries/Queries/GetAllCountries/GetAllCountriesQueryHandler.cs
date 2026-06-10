using ErrorOr;
using MasterData.Application.Countries.Models;
using MasterData.Application.Countries.Queries.GetPagedCountry;
using MasterData.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Queries.GetAllCountries
{
    internal sealed class GetAllCountriesQueryHandler
    : IRequestHandler<GetAllCountriesQuery, ErrorOr<List<CountryResponse>>>
    {
        private readonly ICountryRepository _repository;

        public GetAllCountriesQueryHandler(ICountryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<List<CountryResponse>>> Handle(
            GetAllCountriesQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _repository.GetAllAsync(cancellationToken);

            var mappedItems = result.Select(c => new CountryResponse(
                c.Id.Value,
                c.Name
            )).ToList();

            return mappedItems;
        }
    }
}
