using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Queries.Login
{
    public record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<string>>;
}
