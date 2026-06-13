using ErrorOr;
using MasterData.Application.Units.Models;
using MasterData.Domain.Repositories;
using MediatR;

namespace MasterData.Application.Units.Queries.GetAll
{
    internal sealed class GetAllUnitsQueryHandler
        : IRequestHandler<GetAllUnitsQuery, ErrorOr<IReadOnlyList<UnitResponse>>>
    {
        private readonly IUnitRepository _repository;

        public GetAllUnitsQueryHandler(IUnitRepository repository) => _repository = repository;

        public async Task<ErrorOr<IReadOnlyList<UnitResponse>>> Handle(
            GetAllUnitsQuery request,
            CancellationToken cancellationToken)
        {
            var units = await _repository.GetAllAsync(cancellationToken);

            var response = units.Select(u => new UnitResponse(
                u.Id.Value, u.Name, u.Description
            )).ToList();

            return response;
        }
    }
}
