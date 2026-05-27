using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.States.Commands.CreateState
{
    public record CreateStateCommand(
    Guid CountryId,
    string Code,
    string Name
) : IRequest<ErrorOr<Guid>>;
}
