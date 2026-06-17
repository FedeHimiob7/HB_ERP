using ErrorOr;
using MasterData.Application.Cities.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Cities.Queries.GetAll
{
    internal sealed class GetAllCitiesQueryHandler
        : IRequestHandler<GetAllCitiesQuery, ErrorOr<IReadOnlyList<CityResponse>>>
    {
        private readonly ICityRepository _repository;

        public GetAllCitiesQueryHandler(ICityRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<CityResponse>>> Handle(
            GetAllCitiesQuery request,
            CancellationToken cancellationToken)
        {
            var cities = await _repository.GetAllAsync(cancellationToken);

            var response = cities
                .Select(c => new CityResponse(c.Id.Value, c.StateId.Value, c.Name))
                .ToList();

            return response;
        }
    }
}
