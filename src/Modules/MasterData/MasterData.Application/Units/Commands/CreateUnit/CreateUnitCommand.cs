using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Commands.CreateUnit
{
    public record CreateUnitCommand(string Name, string Description) : IRequest<ErrorOr<Guid>>;
}
