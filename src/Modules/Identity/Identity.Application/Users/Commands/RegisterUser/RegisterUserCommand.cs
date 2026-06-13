using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.RegisterUser
{
    public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    List<Guid>? RoleIds = null,
    List<Guid>? PslIds = null
    ) : IRequest<ErrorOr<Guid>>;
}
