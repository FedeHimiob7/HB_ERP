using HB_ERP.SharedKernel.Application.Pagination;
using Identity.Application.Common.Models;

namespace Identity.Application.Users.Queries.GetUsersPaged
{
    public record GetUsersPagedQuery(
    int PageNumber = 1,
    int PageSize = 10) : IRequest<ErrorOr<PagedList<UserResponse>>>;
}
