using ErrorOr;
using MasterData.Application.Cities.Models;
using MasterData.Domain.DomainErrors;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.Cities.Queries.GetById
{
    internal sealed class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, ErrorOr<CityResponse>>
    {
        private readonly ICityRepository _repository;

        public GetCityByIdQueryHandler(ICityRepository repository) => _repository = repository;

        public async Task<ErrorOr<CityResponse>> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            var city = await _repository.GetByIdAsync(CityId.Create(request.Id), cancellationToken);
            if (city is null)
                return CityErrors.NotFound;

            return new CityResponse(city.Id.Value, city.StateId.Value, city.Name);
        }
    }
}
