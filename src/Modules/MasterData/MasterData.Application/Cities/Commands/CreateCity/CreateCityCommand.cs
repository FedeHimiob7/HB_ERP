using ErrorOr;
using MediatR;

namespace MasterData.Application.Cities.Commands.CreateCity
{
    public record CreateCityCommand(Guid StateId, string Name) : IRequest<ErrorOr<Guid>>;
}
