using ErrorOr;
using MasterData.Application.Countries.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Countries.Queries.GetCountryById
{
    internal sealed class GetCountryByIdQueryHandler
     : IRequestHandler<GetCountryByIdQuery, ErrorOr<CountryResponse>>
    {
        private readonly ICountryRepository _repository;

        public GetCountryByIdQueryHandler(ICountryRepository repository)
        {
            _repository = repository;
        }

        public async Task<ErrorOr<CountryResponse>> Handle(
            GetCountryByIdQuery request,
            CancellationToken cancellationToken)
        {
            var countryId = CountryId.Create(request.Id);

            var country = await _repository.GetByIdAsync(countryId, cancellationToken);

            if (country is null)
            {
                return Error.NotFound(
                    code: "Country.NotFound",
                    description: "El país solicitado no existe.");
            }

            return new CountryResponse(
                country.Id.Value,
                country.Name);
        }
    }
}
