using ErrorOr;
using MasterData.Application.Cities.Models;
using MediatR;

namespace MasterData.Application.Cities.Queries.GetAll
{
    public record GetAllCitiesQuery() : IRequest<ErrorOr<IReadOnlyList<CityResponse>>>;
}
