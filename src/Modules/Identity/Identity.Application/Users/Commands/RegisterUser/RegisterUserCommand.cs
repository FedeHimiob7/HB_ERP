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
    string Password
    ) : IRequest<ErrorOr<Guid>>;
}
