using ErrorOr;
using MediatR;

namespace MasterData.Application.Cities.Commands.DeactivateCity
{
    public record DeactivateCityCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
