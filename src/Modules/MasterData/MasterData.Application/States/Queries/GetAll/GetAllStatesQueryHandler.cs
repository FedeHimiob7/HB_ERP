using ErrorOr;
using MasterData.Application.States.Models;
using MasterData.Domain.Repositories;
using MasterData.Domain.VO;
using MediatR;

namespace MasterData.Application.States.Queries.GetAll
{
    internal sealed class GetAllStatesQueryHandler
        : IRequestHandler<GetAllStatesQuery, ErrorOr<IReadOnlyList<StateResponse>>>
    {
        private readonly IStateRepository _repository;

        public GetAllStatesQueryHandler(IStateRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<StateResponse>>> Handle(
            GetAllStatesQuery request,
            CancellationToken cancellationToken)
        {
            CountryId? countryId = request.CountryId.HasValue
                ? CountryId.Create(request.CountryId.Value)
                : null;

            var states = await _repository.GetAllAsync(countryId, cancellationToken);

            var response = states
                .Select(s => new StateResponse(s.Id.Value, s.CountryId.Value, s.Code, s.Name))
                .ToList();

            return response;
        }
    }
}
