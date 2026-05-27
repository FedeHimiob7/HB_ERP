using ErrorOr;
using MasterData.Application.States.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Commands.UpdateState
{
    public record UpdateStateCommand(
    Guid Id,
    Guid CountryId,
    string Code,
    string Name
) : IRequest<ErrorOr<StateResponse>>;
}
