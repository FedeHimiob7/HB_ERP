using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Commands.RegisterRole
{
    public sealed record RegisterRoleCommand(
        string Name,
        List<Guid>? ActionIds
    ) : IRequest<ErrorOr<Guid>>;
}
