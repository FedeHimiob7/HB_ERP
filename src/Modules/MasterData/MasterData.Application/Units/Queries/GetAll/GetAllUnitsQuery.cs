using ErrorOr;
using MasterData.Application.Units.Models;
using MediatR;

namespace MasterData.Application.Units.Queries.GetAll
{
    public record GetAllUnitsQuery() : IRequest<ErrorOr<IReadOnlyList<UnitResponse>>>;
}
