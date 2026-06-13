using HB_ERP.SharedKernel.Application.Pagination;
using Identity.Application.Common.Models;

namespace Identity.Application.SystemActions.Queries.GetSystemActionsPaged
{
    public sealed record GetSystemActionsPagedQuery(
        int PageNumber = 1,
        int PageSize = 10) : IRequest<ErrorOr<PagedList<SystemActionResponse>>>;
}
