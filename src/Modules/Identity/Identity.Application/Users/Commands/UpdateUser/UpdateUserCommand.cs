using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.UpdateUser
{
    public sealed record UpdateUserCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    IEnumerable<Guid> RoleIds
    ) : IRequest<ErrorOr<Guid>>;
}
