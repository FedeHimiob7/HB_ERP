using Identity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Users.Queries.GetUsersPaged
{
    public record GetUsersPagedQuery(
    int PageNumber = 1,
    int PageSize = 10) : IRequest<ErrorOr<PagedList<UserResponse>>>;
}
