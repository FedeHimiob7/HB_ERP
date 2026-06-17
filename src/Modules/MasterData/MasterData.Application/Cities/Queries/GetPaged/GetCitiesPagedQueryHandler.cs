using ErrorOr;
using MasterData.Application.Cities.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Cities.Queries.GetPaged
{
    internal sealed class GetCitiesPagedQueryHandler : IRequestHandler<GetCitiesPagedQuery, ErrorOr<PagedCitiesResult>>
    {
        private readonly ICityRepository _repository;

        public GetCitiesPagedQueryHandler(ICityRepository repository) => _repository = repository;

        public async Task<ErrorOr<PagedCitiesResult>> Handle(GetCitiesPagedQuery request, CancellationToken cancellationToken)
        {
            var (cities, totalCount) = await _repository.GetPagedAsync(request.Filter, cancellationToken);

            var items = cities
                .Select(c => new CityResponse(c.Id.Value, c.StateId.Value, c.Name))
                .ToList();

            return new PagedCitiesResult(items, totalCount);
        }
    }
}
