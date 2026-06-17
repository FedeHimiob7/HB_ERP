using ErrorOr;
using MasterData.Application.Cities.Models;
using MediatR;

namespace MasterData.Application.Cities.Queries.GetById
{
    public record GetCityByIdQuery(Guid Id) : IRequest<ErrorOr<CityResponse>>;
}
