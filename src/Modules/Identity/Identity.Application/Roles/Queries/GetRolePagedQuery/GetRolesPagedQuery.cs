using HB_ERP.SharedKernel.Application.Pagination;
using Identity.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Queries.GetRolePagedQuery
{
    public record GetRolesPagedQuery(
    int PageNumber = 1,
    int PageSize = 10) : IRequest<ErrorOr<PagedList<RoleResponse>>>;
}
