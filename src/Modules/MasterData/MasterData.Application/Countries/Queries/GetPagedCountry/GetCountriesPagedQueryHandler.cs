using ErrorOr;
using MasterData.Application.Countries.Models;
using MasterData.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Queries.GetPagedCountry
{
    internal sealed class GetCountriesPagedQueryHandler
    : IRequestHandler<GetCountriesPagedQuery, ErrorOr<PagedCountriesResult>>
    {
        private readonly ICountryRepository _repository;

        public GetCountriesPagedQueryHandler(ICountryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<PagedCountriesResult>> Handle(
            GetCountriesPagedQuery request,
            CancellationToken cancellationToken)
        {
            var (countries, totalCount) = await _repository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                cancellationToken);

            var mappedItems = countries.Select(c => new CountryResponse(
                c.Id.Value,
                c.Name
            )).ToList();

            return new PagedCountriesResult(mappedItems, totalCount);
        }
    }
}
