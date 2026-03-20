using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.DeleteUser
{
    public record DeleteUserCommand(Guid UserId) : IRequest<ErrorOr<Success>>;
}
