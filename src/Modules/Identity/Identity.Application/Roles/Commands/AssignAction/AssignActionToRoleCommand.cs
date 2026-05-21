using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Commands.AssignAction
{
    public sealed record AssignActionToRoleCommand(
    Guid RoleId,
    Guid ActionId) : IRequest<ErrorOr<Success>>;
}
