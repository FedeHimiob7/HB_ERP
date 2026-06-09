using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Commands.DeleteState
{
    public record DeactivateStateCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
