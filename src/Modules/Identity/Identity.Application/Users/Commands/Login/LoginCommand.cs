using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Commands.Login
{
    public record LoginCommand(
    string Email,
    string Password) : IRequest<ErrorOr<string>>;
}
