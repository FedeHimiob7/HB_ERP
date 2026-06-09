using HB_ERP.SharedKernel.Application.Pagination;
using Identity.Application.Common.Models;
using Identity.Application.Users.Queries.GetUsersPaged;
using Identity.Domain;
using Identity.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Roles.Queries.GetRolePagedQuery
{
    public sealed class GetRolesPagedQueryHandler(IRoleRepository roleRepository)
      : IRequestHandler<GetRolesPagedQuery, ErrorOr<PagedList<RoleResponse>>>
    {
        public async Task<ErrorOr<PagedList<RoleResponse>>> Handle(
            GetRolesPagedQuery request,
            CancellationToken cancellationToken)
        {
            var (roles, totalCount) = await roleRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            var roleResponses = roles.Select(user => new RoleResponse(
                user.Id.Value,
                user.Name,
                user.ActionIds.Select(r => r.Value).ToList()
            )).ToList();

            return new PagedList<RoleResponse>(roleResponses, totalCount, request.PageNumber, request.PageSize);

        }
    }
}
