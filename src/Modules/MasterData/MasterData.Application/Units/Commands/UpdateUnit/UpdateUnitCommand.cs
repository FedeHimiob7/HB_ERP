using ErrorOr;
using MasterData.Application.Units.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterData.Application.Units.Commands.UpdateUnit
{
    public record UpdateUnitCommand(Guid Id, string Name, string Description) : IRequest<ErrorOr<UnitResponse>>;
}
