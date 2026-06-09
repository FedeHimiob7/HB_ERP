using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Commands.DeleteUnit
{
    public record DeactivateUnitCommand(Guid Id) : IRequest<ErrorOr<Success>>;
}
