using ErrorOr;
using MasterData.Application.Cities.Models;
using MediatR;

namespace MasterData.Application.Cities.Commands.UpdateCity
{
    public record UpdateCityCommand(Guid Id, Guid StateId, string Name) : IRequest<ErrorOr<CityResponse>>;
}
