using ErrorOr;
using MasterData.Application.States.Models;
using MediatR;

namespace MasterData.Application.States.Queries.GetAll
{
    public record GetAllStatesQuery(Guid? CountryId = null) : IRequest<ErrorOr<IReadOnlyList<StateResponse>>>;
}
